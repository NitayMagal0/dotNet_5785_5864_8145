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
        if (call == null)
        {
            throw new BoNullReferenceException("Call cannot be null");
        }

        if (call.Latitude < -90 || call.Latitude > 90)
        {
            throw new BoInvalidFormatException("Latitude must be between -90 and 90");
        }

        if (call.Longitude < -180 || call.Longitude > 180)
        {
            throw new BoInvalidFormatException("Longitude must be between -180 and 180");
        }

        if (call.OpeningTime == default)
        {
            throw new BoInvalidFormatException("Opening time must be set");
        }

        if (call.MaxCompletionTime.HasValue && call.MaxCompletionTime.Value <= call.OpeningTime)
        {
            throw new BoInvalidFormatException("Max completion time must be after the opening time");
        }
        // Convert BO.Call to DO.Call
        var doCall = new DO.Call
        {
            Id = call.Id,
            CallType = CallManager.MapCallType(call.CallType),
            Description = call.Description,
            FullAddress = call.FullAddress,
            Latitude = call.Latitude ?? 0.0,
            Longitude = call.Longitude ?? 0.0,
            OpeningTime = call.OpeningTime,
            MaxCompletionTime = call.MaxCompletionTime
        };

        try
        {
            // Attempt to add the new call to the data layer
            _dal.Call.Create(doCall);
        }
        catch (Exception ex)
        {
            // Catch any exceptions from the data layer and re-throw with a suitable message
            throw new InvalidOperationException("Failed to add the call", ex);
        }
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
            // Fetch the call from the data layer
            var doCall = _dal.Call.Read(callId);

            if (doCall == null)
            {
                throw new ArgumentException("Call not found");
            }

            // Convert DO.Call to BO.Call
            var boCall = CallManager.ConvertCallToBO(doCall);

            // Check if the call is in the open status and has never been assigned
            var assignments = _dal.Assignment.ReadAll().Where(a => a.CallId == callId);
            if (boCall.Status != BO.CallStatus.Open || assignments.Any())
            {
                throw new InvalidOperationException("Call cannot be deleted. It is either not open or has been assigned.");
            }

            // Attempt to delete the call
            _dal.Call.Delete(callId);
        }
        catch (Exception ex)
        {
            // Catch any exceptions from the data layer and re-throw with a suitable message
            throw new InvalidOperationException("Failed to delete the call", ex);
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
        // Fetch all calls
        var calls = _dal.Call.ReadAll();

        // Group calls by their status and count them
        var callCounts = calls
            .GroupBy(call => (int)CallManager.GetCallStatus(call.Id))
            .Select(group => new { Status = group.Key, Count = group.Count() })
            .ToDictionary(g => g.Status, g => g.Count);

        // Initialize an array to hold the counts for each status
        int[] statusCounts = new int[Enum.GetValues(typeof(CallStatus)).Length];

        // Populate the array with the counts
        foreach (var kvp in callCounts)
        {
            statusCounts[kvp.Key] = kvp.Value;
        }

        return statusCounts;
    }

    public Call GetCallDetails(int callId)
    {
        // Request the data layer to get details about the call
        var doCall = _dal.Call.Read(callId);

        // If the call does not exist, throw an exception
        if (doCall == null)
        {
            throw new Exception($"Call with ID {callId} does not exist.");
        }

        // Convert DO.Call to BO.Call using CallManager.ConvertCallToBO
        var boCall = CallManager.ConvertCallToBO(doCall);

        return boCall;
    }

    public IEnumerable<CallInList> GetFilteredAndSortedCalls(CallType? filterField, object? filterValue, Enum? sortField)
    {
        // Fetch all calls
        var calls = _dal.Call.ReadAll();

        // Convert DO.Call to BO.Call using CallManager.ConvertCallToBO
        var boCalls = calls.Select(CallManager.ConvertCallToBO);

        // Convert BO.Call to BO.CallInList
        var callInList = boCalls.Select(call => new CallInList
        {
            CallId = call.Id,
            CallType = call.CallType,
            LastVolunteer = call.CallAssigns?.LastOrDefault()?.VolunteerName,
            Status = call.Status,
            AssignmentsCount = call.CallAssigns?.Count ?? 0
        });

        // Apply filtering if filterField and filterValue are not null
        if (filterField.HasValue && filterValue != null)
        {
            callInList = callInList.Where(call =>
            {
                var property = typeof(CallInList).GetProperty(filterField.ToString());
                return property != null && property.GetValue(call)?.Equals(filterValue) == true;
            });
        }

        // Apply sorting
        if (sortField != null)
        {
            var property = typeof(CallInList).GetProperty(sortField.ToString());
            if (property != null)
            {
                callInList = callInList.OrderBy(call => property.GetValue(call));
            }
        }
        else
        {
            // Default sorting by CallId
            callInList = callInList.OrderBy(call => call.CallId);
        }

        return callInList;
    }

    public IEnumerable<ClosedCallInList> GetVolunteerClosedCallsHistory(int volunteerId, CallType? callTypeFilter, Enum? sortField)
    {
        // Retrieve all assignments for the given volunteer
        var assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId &&
                                                       (a.AssignmentStatus == DO.AssignmentStatus.Completed ||
                                                        a.AssignmentStatus == DO.AssignmentStatus.CancelledByUser ||
                                                        a.AssignmentStatus == DO.AssignmentStatus.CancelledByAdmin ||
                                                        a.AssignmentStatus == DO.AssignmentStatus.ExpiredCancellation)).ToList();

        // Retrieve all calls related to the assignments
        var calls = assignments.Select(a => _dal.Call.Read(a.CallId)).ToList();

        // Filter calls by call type if a filter is provided
        if (callTypeFilter.HasValue)
        {
            calls = calls.Where(c => c.CallType == (DO.CallType)callTypeFilter.Value).ToList();
        }

        // Convert DO.Call to BO.ClosedCallInList
        var closedCalls = calls.Select(c => new BO.ClosedCallInList
        {
            Id = c.Id,
            CallType = (BO.CallType)c.CallType,
            FullAddress = c.FullAddress,
            OpeningTime = c.OpeningTime,
            EntryTime = assignments.First(a => a.CallId == c.Id).AdmissionTime,
            FinishTime = assignments.First(a => a.CallId == c.Id).ActualEndTime,
            AssignmentStatus = (BO.AssignmentStatus?)assignments.First(a => a.CallId == c.Id).AssignmentStatus
        }).ToList();

        // Sort the list by the specified field or by call number if no sort field is provided
        if (sortField == null)
        {
            closedCalls = closedCalls.OrderBy(c => c.Id).ToList();
        }
        else
        {
            closedCalls = closedCalls.OrderBy(c => c.GetType().GetProperty(sortField.ToString()).GetValue(c)).ToList();
        }

        return closedCalls;
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
        // Check the correctness of all values in terms of format
        if (call.MaxCompletionTime <= call.OpeningTime)
        {
            throw new ArgumentException("Maximum completion time must be greater than the opening time.");
        }

        // Assuming a method to validate and get coordinates for the address
        try
        {
            (double x, double y) = Tools.GetCoordinates(call.FullAddress);
            call.Latitude = x;
            call.Longitude = y;
        }
        catch
        {
            throw new BO.BoInvalidFormatException("Invalid address.");
        }

        // Create DO.Call from BO.Call
        var doCall = new DO.Call
        {
            Id = call.Id,
            CallType = CallManager.MapCallType(call.CallType),
            Description = call.Description,
            FullAddress = call.FullAddress,
            Latitude = call.Latitude ?? 0.0,
            Longitude = call.Longitude ?? 0.0,
            OpeningTime = call.OpeningTime,
            MaxCompletionTime = call.MaxCompletionTime
        };

        try
        {
            // Attempt to update the call in the data layer
            _dal.Call.Update(doCall);
        }
        catch (Exception ex)
        {
            // Catch the exception and re-throw a suitable exception towards the display layer
            throw new Exception($"Failed to update call with ID {call.Id}.", ex);
        }
    }
}
