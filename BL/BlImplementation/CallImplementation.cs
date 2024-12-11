using DO;
using Helpers;

namespace BlImplementation;
using BlApi;
using BO;
using System;
using System.Collections.Generic;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void AddCall(Call call)
    {
        // Validate the call object
        if (call == null)
        {
            throw new ArgumentNullException(nameof(call), "Call object cannot be null.");
        }

        if (call.MaxCompletionTime <= call.OpeningTime)
        {
            throw new ArgumentException("Maximum completion time must be greater than the opening time.");
        }

        if (string.IsNullOrWhiteSpace(call.FullAddress))
        {
            throw new ArgumentException("Full address cannot be empty.");
        }

        // Validate the address and update latitude and longitude
        try
        {
            var (latitude, longitude) = Helpers.Tools.GetCoordinates(call.FullAddress);
            call.Latitude = latitude;
            call.Longitude = longitude;
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Invalid address. Please provide a valid address.", ex);
        }

        // Convert BO.Call to DO.Call
        var callEntity = new DO.Call
        {
            Id = call.Id,
            CallType = (DO.CallType)call.CallType,
            Description = call.Description,
            FullAddress = call.FullAddress,
            Latitude = call.Latitude ?? 0.0, // Explicit conversion
            Longitude = call.Longitude ?? 0.0, // Explicit conversion
            OpeningTime = call.OpeningTime,
            MaxCompletionTime = call.MaxCompletionTime,
            Status = (DO.CallStatus)call.Status
        };
        _dal.Call.Create(callEntity);
    }

    public void AssignCallToVolunteer(int volunteerId, int callId)
    {
        try
        {
            // Fetch the call from the data layer
            var call = _dal.Call.Read(callId);
            if (call == null)
            {
                throw new BO.BoDoesNotExistException($"Call with ID {callId} does not exist.");
            }

            // Check if the call has not expired
            if (call.Status.ToString() == "Expired" )
            {
                throw new InvalidOperationException("The call has expired");
            }

            // Check if the call is not already being handled
            var assignments = _dal.Assignment.ReadAll();
            foreach (var assignment in assignments)
            {
                if (assignment.CallId == callId)
                {
                    throw new InvalidOperationException("The call is already being handled by another volunteer");
                }
            }

            // Create a new assignment entity
            var newAssignment = new DO.Assignment
            {
                CallId = callId,
                VolunteerId = volunteerId,
                AdmissionTime = ClockManager.Now,
                ActualEndTime = null,
                AssignmentStatus = null
            };

            // Add the new assignment to the data layer
            _dal.Assignment.Create(newAssignment);
        }
        catch (Exception ex)
        {
            // Catch any exceptions from the data layer and re-throw them with an appropriate message
            throw new ApplicationException("An error occurred while assigning the call to the volunteer", ex);
        }
    }

    public void CancelAssignment(int requesterId, int assignmentId)
    {
        try
        {
            // Fetch the assignment from the data layer
            var assignment = _dal.Assignment.Read(assignmentId);
            if (assignment == null)
            {
                throw new BO.BoDoesNotExistException($"Assignment with ID {assignmentId} does not exist.");
            }

            // Check for cancellation permission
            var requester = _dal.Volunteer.Read(requesterId);
            if (requester == null)
            {
                throw new BO.BoDoesNotExistException($"Volunteer with ID {assignmentId} does not exist.");
            }

            bool isAdmin = requester.Role == DO.Role.Manager;
            bool isVolunteer = assignment.VolunteerId == requesterId;

            if (!isAdmin && !isVolunteer)
            {
                throw new UnauthorizedAccessException("The requester is not authorized to cancel this assignment");
            }

            // Check if the assignment is open and not processed
            if (assignment.AssignmentStatus != null || assignment.ActualEndTime != null)
            {
                throw new InvalidOperationException("The assignment is not open or has already been processed/canceled/expired");
            }

            // Update the assignment entity
            Assignment newAssignment = new Assignment(
                assignment.Id,
                assignment.CallId,
                assignment.VolunteerId,
                assignment.AdmissionTime,
                Helpers.ClockManager.Now,
                isVolunteer ? DO.AssignmentStatus.CancelledByUser : DO.AssignmentStatus.CancelledByAdmin);

            // Apply the update to the data layer
            _dal.Assignment.Update(newAssignment);
        }
        catch (Exception ex)
        {
            // Catch any exceptions from the data layer and re-throw them with an appropriate message
            throw new ApplicationException("An error occurred while canceling the assignment", ex);
        }
    }

    public void DeleteCall(int callId)
    {
        try
        {
            // Retrieve the call from the data layer
            var call = _dal.Call.Read(callId);
            if (call == null)
            {
                throw new BO.BoDoesNotExistException($"Call with ID {callId} does not exist.");
            }

            // Check if the call is in the open status
            if (call.Status != DO.CallStatus.Open)
            {
                throw new BO.BoInvalidOperationException("Only calls in the open status can be deleted.");
            }

            // Check if the call has never been assigned to any volunteer
            var assignments = _dal.Assignment.ReadAll().Where(a => a.CallId == callId).ToList();
            if (assignments.Any())
            {
                throw new BO.BoInvalidOperationException("Calls that have been assigned to volunteers cannot be deleted.");
            }

            // Attempt to delete the call from the data layer
            _dal.Call.Delete(callId);
        }
        catch (DalDoesNotExistException ex)
        {
            throw new BO.BoDoesNotExistException($"Call with ID {callId} does not exist.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BoInvalidOperationException("An error occurred while attempting to delete the call.", ex);
        }
    }

    public IEnumerable<OpenCallInList> GetAvailableOpenCallsForVolunteer(int volunteerId, CallType? callTypeFilter, Enum? sortField)
    {
        // Retrieve the volunteer's details to get their location
        var volunteer = _dal.Volunteer.Read(volunteerId);
        if (volunteer == null)
        {
            throw new ArgumentException("Invalid volunteer ID");
        }

        // Retrieve all open calls from the data layer
        var openCalls = _dal.Call.ReadAll()
            .Where(call => call.Status == DO.CallStatus.Open || call.Status == DO.CallStatus.OpenAtRisk)
            .Select(call => new BO.OpenCallInList
            {
                Id = call.Id,
                CallType = (BO.CallType)call.CallType,
                FullAddress = call.FullAddress,
                OpeningTime = call.OpeningTime,
                MaxCompletionTime = call.MaxCompletionTime,
                DistanceFromVolunteer = Tools.CalculateDistance(volunteer.Latitude ?? 0.0, volunteer.Longitude ?? 0.0, call.Latitude, call.Longitude)
            });

        // Filter by call type if provided
        if (callTypeFilter.HasValue)
        {
            openCalls = openCalls.Where(call => call.CallType == callTypeFilter.Value);
        }

        // Sort by the specified field if provided, otherwise sort by call number (Id)
        if (sortField != null)
        {
            openCalls = sortField.ToString() switch
            {
                nameof(BO.OpenCallInList.CallType) => openCalls.OrderBy(call => call.CallType),
                nameof(BO.OpenCallInList.FullAddress) => openCalls.OrderBy(call => call.FullAddress),
                nameof(BO.OpenCallInList.OpeningTime) => openCalls.OrderBy(call => call.OpeningTime),
                nameof(BO.OpenCallInList.MaxCompletionTime) => openCalls.OrderBy(call => call.MaxCompletionTime),
                nameof(BO.OpenCallInList.DistanceFromVolunteer) => openCalls.OrderBy(call => call.DistanceFromVolunteer),
                _ => openCalls.OrderBy(call => call.Id)
            };
        }
        else
        {
            openCalls = openCalls.OrderBy(call => call.Id);
        }

        return openCalls.ToList();
    }

    public int[] GetCallCountsByStatus()
    {
        // Retrieve all calls from the data access layer
        var calls = _dal.Call.ReadAll().ToList();

        // Group calls by their status and count the number of calls in each group
        var groupedCalls = calls
            .GroupBy(call => call.Status)
            .Select(group => new { Status = group.Key, Count = group.Count() })
            .ToList();

        // Initialize an array to hold the counts for each status
        int[] callCounts = new int[Enum.GetValues(typeof(CallStatus)).Length];

        // Populate the array with the counts from the grouped calls
        foreach (var group in groupedCalls)
        {
            callCounts[(int)group.Status] = group.Count;
        }

        return callCounts;
    }

    public Call GetCallDetails(int callId)
    {
        // Retrieve the call details from the data layer
        var callEntity = _dal.Call.Read(callId);
        if (callEntity == null)
        {
            throw new BoDoesNotExistException($"Call with ID {callId} does not exist.");
        }

        // Retrieve the list of assignments for the call
        var assignments = _dal.Assignment.ReadAll(a => a.CallId == callId).ToList();

        // Convert the data layer call entity to the business object call entity
        var call = new Call
        {
            Id = callEntity.Id,
            CallType = (CallType)callEntity.CallType,
            Description = callEntity.Description,
            FullAddress = callEntity.FullAddress,
            Latitude = callEntity.Latitude,
            Longitude = callEntity.Longitude,
            OpeningTime = callEntity.OpeningTime,
            MaxCompletionTime = callEntity.MaxCompletionTime,
            Status = (CallStatus)callEntity.Status,
            CallAssigns = assignments.Select(a => new CallAssignInList
            {
                VolunteerId = a.VolunteerId,
                VolunteerName = _dal.Volunteer.Read(a.VolunteerId)?.FullName,
                EntryTime = a.AdmissionTime,
                FinishTime = a.ActualEndTime,
                AssignmentStatus = (AssignmentStatus?)a.AssignmentStatus
            }).ToList()
        };

        return call;
    }

    public IEnumerable<CallInList> GetFilteredAndSortedCalls(CallType? filterField, object? filterValue, Enum? sortField)
    {
        // Retrieve all calls from the data access layer
        var calls = _dal.Call.ReadAll().ToList();

        // Retrieve all assignments from the data access layer
        var assignments = _dal.Assignment.ReadAll().ToList();

        // Filter the calls if filterField and filterValue are provided
        if (filterField != null && filterValue != null)
        {
            calls = calls.Where(call =>
            {
                var property = typeof(Call).GetProperty(filterField.ToString());
                return property != null && property.GetValue(call)?.Equals(filterValue) == true;
            }).ToList();
        }

        // Sort the calls if sortField is provided
        if (sortField != null)
        {
            calls = calls.OrderBy(call =>
            {
                var property = typeof(Call).GetProperty(sortField.ToString());
                return property?.GetValue(call);
            }).ToList();
        }
        else
        {
            // Default sorting by CallId
            calls = calls.OrderBy(call => call.Id).ToList();
        }

        // Ensure each call appears only once with its last assignment (if any)
        var uniqueCalls = calls.GroupBy(call => call.Id)
            .Select(group => group.Last())
            .ToList();

        // Convert DO.Call to BO.CallInList
        var callInList = uniqueCalls.Select(call =>
        {
            var lastAssignment = assignments.LastOrDefault(a => a.CallId == call.Id);
            return new CallInList
            {
                Id = call.Id,
                CallId = call.Id,
                CallType = (CallType)call.CallType,
                OpeningTime = call.OpeningTime,
                RemainingTime = call.MaxCompletionTime.HasValue ? call.MaxCompletionTime.Value - DateTime.Now : (TimeSpan?)null,
                LastVolunteer = lastAssignment.VolunteerId.ToString(),
                TotalTime = lastAssignment.ActualEndTime.HasValue == true ? lastAssignment.ActualEndTime.Value - lastAssignment.AdmissionTime : (TimeSpan?)null,
                Status = (CallStatus?)call.Status,
                AssignmentsCount = assignments.Count(a => a.CallId == call.Id)
            };
        }).ToList();

        return callInList;
    }

    public IEnumerable<ClosedCallInList> GetVolunteerClosedCallsHistory(int volunteerId, CallType? callTypeFilter, Enum? sortField)
    {
        // Retrieve all closed calls for the volunteer from the data layer
        var closedCalls = _dal.Call.ReadAll()
            .Where(call => call.Status == DO.CallStatus.Completed)
            .Select(call => new
            {
                Call = call,
                Assignment = _dal.Assignment.ReadAll()
                    .FirstOrDefault(a => a.CallId == call.Id && a.VolunteerId == volunteerId)
            })
            .Where(ca => ca.Assignment != null)
            .Select(ca => new BO.ClosedCallInList
            {
                Id = ca.Call.Id,
                CallType = (BO.CallType)ca.Call.CallType,
                FullAddress = ca.Call.FullAddress,
                OpeningTime = ca.Call.OpeningTime,
                EntryTime = ca.Assignment.AdmissionTime,
                FinishTime = ca.Assignment.ActualEndTime,
                AssignmentStatus = (BO.AssignmentStatus)ca.Assignment.AssignmentStatus
            });

        // Filter by call type if provided
        if (callTypeFilter.HasValue)
        {
            closedCalls = closedCalls.Where(call => call.CallType == callTypeFilter.Value);
        }

        // Sort by the specified field if provided, otherwise sort by call number (Id)
        if (sortField != null)
        {
            closedCalls = sortField.ToString() switch
            {
                nameof(BO.ClosedCallInList.CallType) => closedCalls.OrderBy(call => call.CallType),
                nameof(BO.ClosedCallInList.FullAddress) => closedCalls.OrderBy(call => call.FullAddress),
                nameof(BO.ClosedCallInList.OpeningTime) => closedCalls.OrderBy(call => call.OpeningTime),
                nameof(BO.ClosedCallInList.EntryTime) => closedCalls.OrderBy(call => call.EntryTime),
                nameof(BO.ClosedCallInList.FinishTime) => closedCalls.OrderBy(call => call.FinishTime),
                nameof(BO.ClosedCallInList.AssignmentStatus) => closedCalls.OrderBy(call => call.AssignmentStatus),
                _ => closedCalls.OrderBy(call => call.Id)
            };
        }
        else
        {
            closedCalls = closedCalls.OrderBy(call => call.Id);
        }

        return closedCalls.ToList();
    }

    public void MarkAssignmentAsCompleted(int volunteerId, int assignmentId)
    {
        try
        {
            // Fetch the assignment from the data layer
            var assignment = _dal.Assignment.Read(assignmentId);
            if (assignment == null)
            {
                throw new BoDoesNotExistException($"Assignment with ID: {assignmentId} does not exist");
            }

            // Check if the requester is the volunteer for whom the assignment is registered
            if (assignment.VolunteerId != volunteerId)
            {
                throw new UnauthorizedAccessException("The requester is not authorized to complete this assignment");
            }

            // Check if the assignment is open
            if (assignment.AssignmentStatus != null || assignment.ActualEndTime != null)
            {
                throw new InvalidOperationException("The assignment is not open or has already been completed/canceled/expired");
            }

            Assignment newAssignment = new Assignment(
                assignment.Id,
                assignment.CallId,
                assignment.VolunteerId,
                assignment.AdmissionTime, 
                Helpers.ClockManager.Now,
                DO.AssignmentStatus.Completed);

            // Apply the update to the data layer
            _dal.Assignment.Update(newAssignment);
        }
        catch (Exception ex)
        {
            // Catch any exceptions from the data layer and re-throw them with an appropriate message
            throw new ApplicationException("An error occurred while marking the assignment as completed", ex);
        }
    }

    public void UpdateCall(Call call)
    {
        if (call == null)
        {
            throw new BoNullReferenceException($"Call can't be null.");
        }

        if (call.MaxCompletionTime <= call.OpeningTime)
        {
            throw new ArgumentException("Maximum completion time must be greater than the opening time.");
        }

        if (string.IsNullOrWhiteSpace(call.FullAddress))
        {
            throw new ArgumentException("Full address cannot be empty.");
        }
        try
        {
            // Validate the address and update latitude and longitude
            var (latitude, longitude) = Helpers.Tools.GetCoordinates(call.FullAddress);
            call.Latitude = latitude;
            call.Longitude = longitude;
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Invalid address. Please provide a valid address.", ex);
        }

        // Convert BO.Call to DO.Call
        var callEntity = new DO.Call
        {
            Id = call.Id,
            CallType = (DO.CallType)call.CallType,
            Description = call.Description,
            FullAddress = call.FullAddress,
            Latitude = call.Latitude ?? 0.0,
            Longitude = call.Longitude ?? 0.0,
            OpeningTime = call.OpeningTime,
            MaxCompletionTime = call.MaxCompletionTime,
            Status = (DO.CallStatus)call.Status
        };

        try
        {
            // Attempt to update the call in the data layer
            _dal.Call.Update(callEntity);
        }
        catch (DalDoesNotExistException ex)
        {
            throw new BO.BoDoesNotExistException($"Call with ID {call.Id} does not exist.", ex);
        }
    }
}
