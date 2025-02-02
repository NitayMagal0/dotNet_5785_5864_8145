using Helpers;
namespace BlImplementation;
using BlApi;
using BO;
using System;
using System.Collections.Generic;
/// <summary>
/// Implementation of the ICall interface for managing Call entities in the BL (Business Logic) layer.
/// </summary>
internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    /// <summary>
    /// Adds a minimal call with the specified details.
    /// </summary>
    /// <param name="CallType">The type of the call.</param>
    /// <param name="Description">The description of the call.</param>
    /// <param name="FullAddress">The full address of the call.</param>
    /// <param name="MaxCompletionTime">The maximum completion time for the call.</param>
    public void MinAddCall(BO.CallType CallType, string Description, string FullAddress, DateTime? MaxCompletionTime)
    {
        // Check if the simulator is running
        AdminManager.ThrowOnSimulatorIsRunning();
        // Validation Checks
        if (CallType == 0)
        {
            throw new BlInvalidFormatException("Please select a valid Call Type.");
        }

        if (string.IsNullOrWhiteSpace(Description))
        {
            throw new BlInvalidFormatException("Please enter a valid Description.");
        }

        if (string.IsNullOrWhiteSpace(FullAddress))
        {
            throw new BlInvalidFormatException("Please enter a valid Full Address.");
        }
        int nextCallId;
        lock (AdminManager.BlMutex)
            nextCallId = _dal.Config.NextCallId;
        DateTime OpeningTime = Helpers.AdminManager.Now;

        if (!MaxCompletionTime.HasValue || MaxCompletionTime <= OpeningTime)
        {
            throw new BlInvalidFormatException("Max completion time must be a valid date and after the opening time.");
        }

        double Latitude, Longitude;
        try
        {
            (Latitude, Longitude) = Helpers.Tools.GetCoordinates(FullAddress);
        }
        catch (Exception ex)
        {
            throw new BlInvalidFormatException($"Invalid address: {ex.Message}");
        }
        DateTime newOpeningTime;
        lock (AdminManager.BlMutex) 
             newOpeningTime = _dal.Config.Clock;

        // Convert BO.Call to DO.Call
        var doCall = new DO.Call
        {
            Id = nextCallId,
            CallType = CallManager.MapCallType(CallType),
            Description = Description,
            FullAddress = FullAddress,
            Latitude = Latitude,
            Longitude = Longitude,
            OpeningTime = newOpeningTime,
            MaxCompletionTime = MaxCompletionTime
        };

        try
        {
            // Attempt to add the new call to the data layer
            lock (AdminManager.BlMutex)
                _dal.Call.Create(doCall);
            CallManager.Observers.NotifyListUpdated();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to add the call: " + ex.Message);
        }
    }


    /// <summary>
    /// Adds a new call.
    /// </summary>
    /// <param name="call">The call entity to add.</param>
    public void AddCall(Call call)
    {
        // Check if the simulator is running
        AdminManager.ThrowOnSimulatorIsRunning();
        if (call == null)
        {
            throw new BlNullReferenceException("Call cannot be null");
        }
        call.OpeningTime = Helpers.AdminManager.Now;
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
            lock (AdminManager.BlMutex)
                _dal.Call.Create(doCall);
            CallManager.Observers.NotifyListUpdated(); //stage 5                                                    
        }
        catch (Exception ex)
        {
            // Catch any exceptions from the data layer and re-throw with a suitable message
            throw new InvalidOperationException("Failed to add the call", ex);
        }
    }
    /// <summary>
    /// Assigns a call to a volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <param name="callId">The ID of the call.</param>
    public void AssignCallToVolunteer(int volunteerId, int callId)
    {
        try
        {
            // Fetch the call from the data layer
            DO.Call call;
            lock (AdminManager.BlMutex)
                call = _dal.Call.Read(callId);
            if (call == null)
            {
                throw new ArgumentException("Call not found");
            }

            // Check if the call has expired
            var systemClock = AdminManager.Now;
            if (call.MaxCompletionTime.HasValue && systemClock > call.MaxCompletionTime.Value)
            {
                throw new InvalidOperationException("The call has expired");
            }

            // Check if the call is already being handled
            IEnumerable<DO.Assignment> assignments;
            lock (AdminManager.BlMutex)
                 assignments = _dal.Assignment.ReadAll().Where(a => a.CallId == callId);
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
            lock (AdminManager.BlMutex)
                _dal.Assignment.Create(newAssignment);
            AssignmentManager.Observers.NotifyListUpdated(); //stage 5                                                    

        }
        catch (Exception ex)
        {
            // Catch any exceptions thrown by the data layer and re-throw them
            throw new InvalidOperationException($"Failed to assign the call to the volunteer {ex.Message}");
        }
    }
    /// <summary>
    /// Cancels an assignment.
    /// </summary>
    /// <param name="requesterId">The ID of the requester.</param>
    /// <param name="callId">The ID of the call.</param>
    public void CancelAssignment(int requesterId, int callId)
    {
        try
        {
            // Fetch the assignment from the data layer using callId
            IEnumerable<DO.Assignment> assignments;
            lock (AdminManager.BlMutex)
                 assignments = _dal.Assignment.ReadAll();
            var assignment = assignments.FirstOrDefault(a => a.CallId == callId);
            if (assignment == null)
            {
                throw new ArgumentException("Assignment not found for the given CallId");
            }

            // Check for cancellation permission
            DO.Volunteer requester;
            lock (AdminManager.BlMutex)
                 requester = _dal.Volunteer.Read(requesterId);
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
                ActualEndTime = AdminManager.Now,
                AssignmentStatus = isVolunteer ? DO.AssignmentStatus.CancelledByUser : DO.AssignmentStatus.CancelledByAdmin
            };
            // Attempt to update the assignment in the data layer
            lock (AdminManager.BlMutex)
                _dal.Assignment.Update(newAssignment);
            AssignmentManager.Observers.NotifyItemUpdated(newAssignment.Id);  //stage 5
            AssignmentManager.Observers.NotifyListUpdated();  //stage 5
        }
        catch (Exception ex)
        {
            // Catch any exceptions thrown by the data layer and re-throw them
            throw new InvalidOperationException($"Failed to cancel the assignment - {ex.Message}");
        }
    }
    /// <summary>
    /// Deletes a call by its ID.
    /// </summary>
    /// <param name="callId">The ID of the call to delete.</param>
    public void DeleteCall(int callId)
    {
        try
        {
            // Fetch the call from the data layer
            DO.Call doCall;
            lock (AdminManager.BlMutex)
                 doCall = _dal.Call.Read(callId);

            if (doCall == null)
            {
                throw new ArgumentException("Call not found");
            }

            // Convert DO.Call to BO.Call
            var boCall = CallManager.ConvertCallToBO(doCall);

            // Check if the call is in the open status and has never been assigned
            IEnumerable<DO.Assignment> assignments;
            lock (AdminManager.BlMutex)
                 assignments = _dal.Assignment.ReadAll().Where(a => a.CallId == callId);
            if ((boCall.Status != BO.CallStatus.Open && boCall.Status != BO.CallStatus.OpenAtRisk) || assignments.Any())
            {
                throw new InvalidOperationException("Call cannot be deleted. It is either not open or has been assigned.");
            }

            // Attempt to delete the call
            _dal.Call.Delete(callId);
            CallManager.Observers.NotifyListUpdated();  //stage 5 
        }
        catch (Exception ex)
        {
            // Catch any exceptions from the data layer and re-throw with a suitable message
            throw new InvalidOperationException(ex.Message);
        }
    }
    /// <summary>
    /// Gets the available open calls for a specific volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <param name="callTypeFilter">The call type filter.</param>
    /// <param name="sortField">The field to sort by.</param>
    /// <returns>An enumerable collection of open calls.</returns>
    public IEnumerable<OpenCallInList> GetAvailableOpenCallsForVolunteer(int volunteerId, CallType? callTypeFilter, Enum? sortField)
    {
        // Retrieve volunteer details to get their location
        DO.Volunteer volunteer;
        lock (AdminManager.BlMutex)
             volunteer = _dal.Volunteer.Read(volunteerId);
        if (volunteer == null)
        {
            throw new InvalidOperationException("Volunteer not found");
        }

        // Retrieve all calls and convert them to BO.Call
        List<BO.Call> allCalls;
        lock (AdminManager.BlMutex)
             allCalls = _dal.Call.ReadAll().Select(CallManager.ConvertCallToBO).ToList();

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
    /// <summary>
    /// Gets the count of calls by their status.
    /// </summary>
    /// <returns>An array of integers representing the count of calls for each status.</returns>
    public int[] GetCallCountsByStatus()
    {
        // Fetch all calls
        IEnumerable<DO.Call> calls;
        lock (AdminManager.BlMutex)
             calls = _dal.Call.ReadAll();

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
    /// <summary>
    /// Gets the details of a specific call by its ID.
    /// </summary>
    /// <param name="callId">The ID of the call.</param>
    /// <returns>The details of the call.</returns>
    public Call GetCallDetails(int callId)
    {
        // Request the data layer to get details about the call
        DO.Call doCall;
        lock (AdminManager.BlMutex)
             doCall = _dal.Call.Read(callId);

        // If the call does not exist, throw an exception
        if (doCall == null)
        {
            throw new Exception($"Call with ID {callId} does not exist.");
        }

        // Convert DO.Call to BO.Call using CallManager.ConvertCallToBO
        BO.Call boCall = CallManager.ConvertCallToBO(doCall);
        return boCall;
    }
    /// <summary>
    /// Gets a call in list by its ID.
    /// </summary>
    /// <param name="callId">The ID of the call.</param>
    /// <returns>The call in list.</returns>
    public CallInList GetCallInListById(int callId)
    {
        // Request the data layer to get details about the call
        DO.Call doCall;
        lock (AdminManager.BlMutex)
            doCall = _dal.Call.Read(callId);

        // If the call does not exist, throw an exception
        if (doCall == null)
        {
            throw new Exception($"Call with ID {callId} does not exist.");
        }

        // Convert DO.Call to BO.Call using CallManager.ConvertCallToBO
        BO.Call boCall = CallManager.ConvertCallToBO(doCall);

        // Convert BO.Call to BO.CallInList
        var callInList = new CallInList
        {
            CallId = boCall.Id,
            CallType = boCall.CallType,
            LastVolunteer = boCall.CallAssigns?.LastOrDefault()?.VolunteerName,
            RemainingTime = boCall.MaxCompletionTime - AdminManager.Now,
            Status = boCall.Status,
            AssignmentsCount = boCall.CallAssigns?.Count ?? 0
        };

        return callInList;
    }

    /// <summary>
    /// Gets a filtered and sorted list of calls.
    /// </summary>
    /// <param name="filterField">The field to filter by.</param>
    /// <param name="filterValue">The value to filter by.</param>
    /// <param name="sortField">The field to sort by.</param>
    /// <returns>An enumerable collection of filtered and sorted calls.</returns>
    public IEnumerable<CallInList> GetFilteredAndSortedCalls(CallType? filterField, CallStatus? filterValue, Enum? sortField)
    {
        // Fetch all calls
        IEnumerable<DO.Call> calls;
        lock (AdminManager.BlMutex)
             calls = _dal.Call.ReadAll();
        
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
        if (filterField!=null&&filterValue==null)
        {
            callInList = callInList.Where(call => call.CallType == filterField);
        }
        if (filterField == null && filterValue != null)
        {
            callInList = callInList.Where(call => call.Status == filterValue);
        }
        // Apply filtering if filterField and filterValue are not null
        if (filterField!=null && filterValue != null)
        {
            callInList = callInList.Where(call =>
            {
                var property = typeof(CallInList).GetProperty(filterField.ToString());
                return property != null && property.GetValue(call)?.Equals(filterValue) == true;
            });
        }

        

        return callInList;
    }
    /// <summary>
    /// Gets the history of closed calls for a specific volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <param name="callTypeFilter">The call type filter.</param>
    /// <param name="sortField">The field to sort by.</param>
    /// <returns>An enumerable collection of closed calls.</returns>
    public IEnumerable<ClosedCallInList> GetVolunteerClosedCallsHistory(int volunteerId, CallType? callTypeFilter, Enum? sortField)
    {
        // Retrieve all assignments for the given volunteer
        IEnumerable<DO.Assignment> assignments;
        lock (AdminManager.BlMutex)
        {
             assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId &&
                                                       (a.AssignmentStatus == DO.AssignmentStatus.Completed ||
                                                        a.AssignmentStatus == DO.AssignmentStatus.CancelledByUser ||
                                                        a.AssignmentStatus == DO.AssignmentStatus.CancelledByAdmin ||
                                                        a.AssignmentStatus == DO.AssignmentStatus.ExpiredCancellation)).ToList();
        }
        // Retrieve all calls related to the assignments
        IEnumerable<DO.Call> calls;
        lock (AdminManager.BlMutex)
             calls = assignments.Select(a => _dal.Call.Read(a.CallId)).ToList();

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
    /// <summary>
    /// Marks an assignment as completed.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <param name="callId">The ID of the call.</param>
    public void MarkAssignmentAsCompleted(int volunteerId, int callId)
    {
        try
        {
            // Fetch all assignments and find the one with the given callId
            IEnumerable<DO.Assignment> assignments;
            lock (AdminManager.BlMutex)
                 assignments = _dal.Assignment.ReadAll();
            var assignment = assignments.FirstOrDefault(a => a.CallId == callId);
            if (assignment == null)
            {
                throw new ArgumentException("Assignment not found for the given CallId");
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
                ActualEndTime = AdminManager.Now,
                AssignmentStatus = DO.AssignmentStatus.Completed
            };
            // Attempt to update the assignment in the data layer
            lock (AdminManager.BlMutex)
                _dal.Assignment.Update(newAssignment);
            AssignmentManager.Observers.NotifyItemUpdated(newAssignment.Id);  //stage 5
            AssignmentManager.Observers.NotifyListUpdated();  //stage 5
        }
        catch (Exception ex)
        {
            // Catch any exceptions thrown by the data layer and re-throw them
            throw new InvalidOperationException($"Failed to complete the assignment: {ex.Message}");
        }
    }
    /// <summary>
    /// Updates an existing call.
    /// </summary>
    /// <param name="call">The call entity with updated values.</param>
    public void UpdateCall(Call call)
    {
        // Check if the simulator is running
        AdminManager.ThrowOnSimulatorIsRunning();
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
            lock (AdminManager.BlMutex)
                _dal.Call.Update(doCall);
            CallManager.Observers.NotifyItemUpdated(doCall.Id);  //stage 5
            CallManager.Observers.NotifyListUpdated();  //stage 5
        }
        catch (Exception ex)
        {
            // Catch the exception and re-throw a suitable exception towards the display layer
            throw new Exception($"Failed to update call with ID {call.Id}.", ex);
        }
    }
    /// <summary>
    /// Gets the calls in progress for a specific volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <returns>An enumerable collection of calls in progress.</returns>
    public IEnumerable<BO.CallInProgress> GetCallsForVolunteer(int volunteerId)
    {
        DO.Volunteer volunteer;
        // Retrieve volunteer details
        lock (AdminManager.BlMutex)
                volunteer = _dal.Volunteer.Read(volunteerId);
       

        if (volunteer == null)
        {
            throw new InvalidOperationException("Volunteer not found.");
        }

        // Fetch all active assignments for the volunteer (e.g., Assignments with Status null or specific active statuses)
        IEnumerable<DO.Assignment> activeAssignments;
        lock (AdminManager.BlMutex)
        {
            activeAssignments = _dal.Assignment.ReadAll(a =>
            a.VolunteerId == volunteerId &&
            a.AssignmentStatus == null // Adjust this condition based on what constitutes an "active" assignment
        ).ToList();

        }

        // If no active assignments, return an empty list
        if (!activeAssignments.Any())
        {
            return Enumerable.Empty<BO.CallInProgress>();
        }

        var callsInProgress = new List<BO.CallInProgress>();

        foreach (var assignment in activeAssignments)
        {
            // Fetch the call associated with the assignment
            DO.Call doCall;
            lock (AdminManager.BlMutex)
                doCall = _dal.Call.Read(assignment.CallId);

            if (doCall == null)
            {
                continue; // Skip if the call doesn't exist
            }

            // Convert DO.Call to BO.Call
            var boCall = CallManager.ConvertCallToBO(doCall);

            // Calculate distance from volunteer to call location
            double distance = 0.0;
            if (volunteer.Latitude.HasValue && volunteer.Longitude.HasValue &&
                boCall.Latitude.HasValue && boCall.Longitude.HasValue)
            {
                distance = Tools.CalculateAirDistance(
                    (volunteer.Latitude.Value, volunteer.Longitude.Value),
                    (boCall.Latitude.Value, boCall.Longitude.Value)
                );
            }

            // Determine the status of the call
            var callStatus = CallManager.GetCallStatus(boCall.Id); // Ensure this method accurately reflects the current status

            // Create a CallInProgress object
            var callInProgress = new BO.CallInProgress
            {
                Id = boCall.Id,
                CallId = boCall.Id, // Assuming CallId is the same as Id; adjust if different
                CallType = boCall.CallType,
                Description = boCall.Description,
                FullAddress = boCall.FullAddress,
                OpeningTime = boCall.OpeningTime,
                MaxCompletionTime = boCall.MaxCompletionTime,
                AdmissionTime = assignment.AdmissionTime,
                DistanceFromVolunteer = distance,
                Status = (BO.CallStatus?)callStatus // Cast if necessary
            };

            callsInProgress.Add(callInProgress);
        }

        return callsInProgress;
    }
    /// <summary>
    /// Gets the nearby open calls for a volunteer within a specified range.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <param name="range">The range to search within.</param>
    /// <param name="distanceType">The type of distance calculation.</param>
    /// <param name="callTypeFilter">The call type filter.</param>
    /// <param name="sortField">The field to sort by.</param>
    /// <returns>An enumerable collection of nearby open calls.</returns>

    public IEnumerable<OpenCallInList> GetNearbyOpenCallsForVolunteer(int volunteerId, double range, DistanceType distanceType, CallType? callTypeFilter, Enum? sortField)
    {
        // Step 1: Retrieve open calls using the existing function
        var availableCalls = GetAvailableOpenCallsForVolunteer(volunteerId, callTypeFilter, sortField).ToList();
        // Step 2: Retrieve volunteer details to get their location
        DO.Volunteer volunteer;
        lock (AdminManager.BlMutex)
              volunteer = _dal.Volunteer.Read(volunteerId);
        if (volunteer == null)
        {
            throw new InvalidOperationException("Volunteer not found");
        }
        var volunteerLocation = (volunteer.Latitude ?? 0.0, volunteer.Longitude ?? 0.0);
        // Step 3: Read all calls from the data layer and convert them to BO.Call
        Dictionary<int, BO.Call> allCalls;
        lock (AdminManager.BlMutex)
        {
            allCalls = _dal.Call.ReadAll()
            .Select(CallManager.ConvertCallToBO)
            .ToDictionary(c => c.Id);
        }
        // Step 4: Filter available calls by range
        var nearbyCalls = new List<OpenCallInList>();
        foreach (var call in availableCalls)
        {
            if (!allCalls.TryGetValue(call.Id, out var boCall))
            {
                continue; // Skip if the call ID isn't found in the converted calls
            }
            var callLocation = (boCall.Latitude ?? 0.0, boCall.Longitude ?? 0.0);
            // Calculate air distance
            var airDistance = Tools.CalculateAirDistance(volunteerLocation, callLocation);
            if (distanceType == DistanceType.AirDistance)
            {
                // For air distance, directly filter based on the range
                if (airDistance <= range)
                {
                    nearbyCalls.Add(call);
                }
            }
            else
            {
                // For driving or walking distance, only check calls within a threshold (e.g., 1.5x range)
                if (airDistance <= range * 1.5)
                {
                    // Calculate real distance using the API
                    var realDistance = Tools.GetResultAsync(volunteerLocation, callLocation, distanceType);
                    // Add the call if the real distance is within range
                    if (realDistance <= range)
                    {
                        nearbyCalls.Add(call);
                    }
                }
            }
        }
        // Step 5: Sort the nearby calls by the specified sort field or default by call ID
        if (sortField != null)
        {
            var property = typeof(OpenCallInList).GetProperty(sortField.ToString());
            if (property != null)
            {
                nearbyCalls = nearbyCalls.OrderBy(c => property.GetValue(c)).ToList();
            }
        }
        return nearbyCalls;
    }
   
    #region Stage 5
    public void AddObserver(Action listObserver) =>
        CallManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
        CallManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
        CallManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
        CallManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5
}
