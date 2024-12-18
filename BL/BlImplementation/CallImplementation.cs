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
            throw new BlNullReferenceException("Call cannot be null");
        }

        if (call.OpeningTime == default)
        {
            throw new BlInvalidFormatException("Opening time must be set");
        }

        if (call.MaxCompletionTime.HasValue && call.MaxCompletionTime.Value <= call.OpeningTime)
        {
            throw new BlInvalidFormatException("Max completion time must be after the opening time");
        }

        double Latitude, Longitude = 0;
        (Latitude, Longitude) = Helpers.Tools.GetCoordinates(call.FullAddress);
        // Convert BO.Call to DO.Call
        var doCall = new DO.Call
        {
            Id = call.Id,
            CallType = CallManager.MapCallType(call.CallType),
            Description = call.Description,
            FullAddress = call.FullAddress,
            Latitude = Latitude,
            Longitude = Longitude,
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
                throw new ArgumentException("Call not found");
            }

            // Check if the call has expired
            var systemClock = ClockManager.Now;
            if (call.MaxCompletionTime.HasValue && systemClock > call.MaxCompletionTime.Value)
            {
                throw new InvalidOperationException("The call has expired");
            }

            // Check if the call is already being handled
            var assignments = _dal.Assignment.ReadAll().Where(a => a.CallId == callId);
            if (assignments.Any(a => a.ActualEndTime == null))
            {
                throw new InvalidOperationException("The call is already being handled by another volunteer");
            }

            // Create a new assignment
            var newAssignment = new DO.Assignment
            {
                CallId = callId,
                VolunteerId = volunteerId,
                AdmissionTime = systemClock,
                ActualEndTime = null,
                AssignmentStatus = null
            };

            // Add the new assignment to the data layer
            _dal.Assignment.Create(newAssignment);
        }
        catch (Exception ex)
        {
            // Catch any exceptions thrown by the data layer and re-throw them
            throw new InvalidOperationException("Failed to assign the call to the volunteer", ex);
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
                throw new ArgumentException("Assignment not found");
            }

            // Check for cancellation permission
            var requester = _dal.Volunteer.Read(requesterId);
            if (requester == null)
            {
                throw new UnauthorizedAccessException("Requester not found");
            }

            bool isAdmin = requester.Role == VolunteerManager.MapRole(Role.Manager);
            bool isVolunteer = assignment.VolunteerId == requesterId;

            if (!isAdmin && !isVolunteer)
            {
                throw new UnauthorizedAccessException("The requester is not authorized to cancel this assignment");
            }

            // Check if the assignment is open (actual end time is still null)
            if (assignment.ActualEndTime != null)
            {
                throw new InvalidOperationException("The assignment is already completed or canceled");
            }

            var newAssignment = new DO.Assignment
            {
                Id = assignment.Id,
                CallId = assignment.CallId,
                VolunteerId = assignment.VolunteerId,
                AdmissionTime = assignment.AdmissionTime,
                ActualEndTime = ClockManager.Now,
                AssignmentStatus = isVolunteer ? DO.AssignmentStatus.CancelledByUser : DO.AssignmentStatus.CancelledByAdmin
            };
            // Attempt to update the assignment in the data layer
            _dal.Assignment.Update(newAssignment);
        }
        catch (Exception ex)
        {
            // Catch any exceptions thrown by the data layer and re-throw them
            throw new InvalidOperationException("Failed to cancel the assignment", ex);
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
        // Retrieve volunteer details to get their location
        var volunteer = _dal.Volunteer.Read(volunteerId);
        if (volunteer == null)
        {
            throw new InvalidOperationException("Volunteer not found");
        }

        // Retrieve all calls and convert them to BO.Call
        var allCalls = _dal.Call.ReadAll().Select(CallManager.ConvertCallToBO).ToList();

        // Filter calls by status
        var openCalls = allCalls.Where(c => c.Status == BO.CallStatus.Open || c.Status == BO.CallStatus.OpenAtRisk).ToList();

        // Filter calls by call type if a filter is provided
        if (callTypeFilter.HasValue)
        {
            openCalls = openCalls.Where(c => c.CallType == callTypeFilter.Value).ToList();
        }

        // Convert BO.Call to BO.OpenCallInList and calculate distance from volunteer
        var openCallInLists = openCalls.Select(c => new BO.OpenCallInList
        {
            Id = c.Id,
            CallType = c.CallType,
            CallTypeDescription = c.Description,
            FullAddress = c.FullAddress,
            OpeningTime = c.OpeningTime,
            MaxCompletionTime = c.MaxCompletionTime,
            DistanceFromVolunteer = Tools.GetResultAsync((volunteer.Latitude ?? 0.0, volunteer.Longitude ?? 0.0), (c.Latitude ?? 0.0, c.Longitude ?? 0.0), calculationType: DistanceType.AirDistance)
        }).ToList();

        // Sort the list by the specified field or by call number if no sort field is provided
        if (sortField == null)
        {
            openCallInLists = openCallInLists.OrderBy(c => c.Id).ToList();
        }
        else
        {
            openCallInLists = openCallInLists.OrderBy(c => c.GetType().GetProperty(sortField.ToString()).GetValue(c)).ToList();
        }

        return openCallInLists;
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
        BO.Call boCall = CallManager.ConvertCallToBO(doCall);

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
                throw new ArgumentException("Assignment not found");
            }

            // Check if the requester is the volunteer for whom the assignment is registered
            if (assignment.VolunteerId != volunteerId)
            {
                throw new UnauthorizedAccessException("The requester is not authorized to complete this assignment");
            }

            // Check if the assignment is open (actual end time is still null)
            if (assignment.ActualEndTime != null)
            {
                throw new InvalidOperationException("The assignment is already completed or canceled");
            }

            // Update the assignment with the end type "treated" and the actual end time
            var newAssignment = new DO.Assignment
            {
                Id = assignment.Id,
                CallId = assignment.CallId,
                VolunteerId = assignment.VolunteerId,
                AdmissionTime = assignment.AdmissionTime,
                ActualEndTime = ClockManager.Now,
                AssignmentStatus = DO.AssignmentStatus.Completed
            };
            // Attempt to update the assignment in the data layer
            _dal.Assignment.Update(newAssignment);
        }
        catch (Exception ex)
        {
            // Catch any exceptions thrown by the data layer and re-throw them
            throw new InvalidOperationException("Failed to complete the assignment", ex);
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
            throw new BO.BlInvalidFormatException("Invalid address.");
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
