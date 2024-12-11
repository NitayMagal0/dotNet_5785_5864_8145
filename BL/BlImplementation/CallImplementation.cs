using DO;
using Helpers;

namespace BlImplementation;
using BlApi;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void AddCall(BO.Call boCall)
    {
        (double latitude, double longitude) = Helpers.Tools.GetCoordinates(boCall.FullAddress);
        DO.Call doCall = new DO.Call(
            boCall.Id,
            (DO.CallType)boCall.CallType, // Convert BO.CallType to DO.CallType
            boCall.Description,
            boCall.FullAddress,
            latitude,
            longitude,
            ClockManager.Now,
            boCall.MaxCompletionTime
        );
        try
        {
            _dal.Call.Create(doCall);
        }
        catch (DO.DalEntityAlreadyExistsException ex)
        {
            throw new BO.BoEntityAlreadyExistsException($"Student with ID={boCall.Id} already exists", ex);
        }
    }

    public void AssignCallToVolunteer(int volunteerId, int callId)
    {
        // Fetch the call details from the data layer
        var call = _dal.Call.Read(callId);
        if (call == null)
        {
            throw new BO.BoDoesNotExistException($"Call with ID={callId} does not exist.");
        }

        // Check if the call has already been processed or has expired
        if (call.MaxCompletionTime.HasValue && call.MaxCompletionTime.Value < ClockManager.Now)
        {
            throw new BO.BoInvalidOperationException("The call has expired.");
        }

        // Fetch all assignments for the call
        var assignments = _dal.Assignment.ReadAll(a => a.CallId == callId);

        // Check if there is any open assignment for the call
        if (assignments.Any(a => a.AssignmentStatus == DO.AssignmentStatus.Open))
        {
            throw new BO.BoInvalidOperationException("The call is already being processed by another volunteer.");
        }

        // Create a new assignment entity
        var newAssignment = new DO.Assignment
        {
            VolunteerId = volunteerId,
            CallId = callId,
            AdmissionTime = ClockManager.Now,
            ActualEndTime = null,
            AssignmentStatus = DO.AssignmentStatus.Open
        };

        try
        {
            _dal.Assignment.Create(newAssignment);
        }
        catch (DO.DalEntityAlreadyExistsException ex)
        {
            throw new BO.BoEntityAlreadyExistsException($"Assignment for call ID={callId} and volunteer ID={volunteerId} already exists.", ex);
        }
    }

    public void CancelAssignment(int requesterId, int assignmentId)
    {
        // Fetch the assignment details from the data layer
        var assignment = _dal.Assignment.Read(assignmentId);
        if (assignment == null)
        {
            throw new BO.BoDoesNotExistException($"Assignment with ID={assignmentId} does not exist.");
        }

        // Fetch the volunteer details to check if the requester is an administrator
        var requester = _dal.Volunteer.Read(requesterId);
        if (requester == null)
        {
            throw new BO.BoDoesNotExistException($"Requester with ID={requesterId} does not exist.");
        }

        // Check for cancellation permission
        if (requester.Role != Role.Manager || assignment.VolunteerId != requesterId)
        {
            throw new BO.BoInvalidOperationException("Requester does not have permission to cancel this assignment.");
        }

        // Check that the assignment is open and has not been processed
        if (assignment.AssignmentStatus != DO.AssignmentStatus.Open || assignment.ActualEndTime != null)
        {
            throw new BO.BoInvalidOperationException("The assignment has already processed or expired.");
        }
        
        DO.Assignment newAssignment = new DO.Assignment
        {
            VolunteerId = assignment.VolunteerId,
            CallId = assignment.CallId,
            AdmissionTime = assignment.AdmissionTime,
            ActualEndTime = ClockManager.Now,
            AssignmentStatus = requester.Role == Role.Manager ? DO.AssignmentStatus.CancelledByAdmin : DO.AssignmentStatus.CancelledByUser
        };

        try
        {
            _dal.Assignment.Update(newAssignment);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BoDoesNotExistException($"Assignment with ID={assignmentId} does not exist.", ex);
        }
    }

    public void DeleteCall(int callId)
    {
        // Fetch the call details from the data layer
        var call = _dal.Call.Read(callId);
        if (call == null)
        {
            throw new BO.BoDoesNotExistException($"Call with ID={callId} does not exist.");
        }

        // Check if the call has never been assigned to any volunteer
        var assignments = _dal.Assignment.ReadAll(a => a.CallId == callId);
        if (assignments.Any())
        {
            throw new BO.BoInvalidOperationException("Calls that have been assigned to volunteers cannot be deleted.");
        }

        try
        {
            // Attempt to request deletion of the call from the data layer
            _dal.Call.Delete(callId);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BoDoesNotExistException($"Call with ID={callId} does not exist.", ex);
        }
    }

    public IEnumerable<BO.OpenCallInList> GetAvailableOpenCallsForVolunteer(int volunteerId, BO.CallType? callTypeFilter, Enum? sortField)
    {
        throw new NotImplementedException();
    }

    public int[] GetCallCountsByStatus()
    {
        // Fetch all calls from the data layer
        var calls = _dal.Call.ReadAll();

        // Group calls by their status and count the number of calls in each group
        var callCountsByStatus = calls
            .GroupBy(call => (int)call.Call)
            .Select(group => new { Status = group.Key, Count = group.Count() })
            .ToDictionary(g => g.Status, g => g.Count);

        // Create an array to hold the counts, initialized to zero
        int maxStatusValue = Enum.GetValues(typeof(DO.AssignmentStatus)).Cast<int>().Max();
        int[] counts = new int[maxStatusValue + 1];

        // Populate the array with the counts from the dictionary
        foreach (var kvp in callCountsByStatus)
        {
            counts[kvp.Key] = kvp.Value;
        }

        return counts;
    }

    public BO.Call GetCallDetails(int callId)
    {
        // Fetch the call details from the data layer
        var doCall = _dal.Call.Read(callId);
        if (doCall == null)
        {
            throw new BO.BoDoesNotExistException($"Call with ID={callId} does not exist.");
        }

        // Fetch the list of assignments for the call
        var doAssignments = _dal.Assignment.ReadAll(a => a.CallId == callId);

        // Create a list of logical entities of the type "Call Assignment in List"
        var callAssignments = doAssignments.Select(a => new BO.CallAssignInList
        {
            VolunteerId = a.VolunteerId,
            VolunteerName = _dal.Volunteer.Read(a.VolunteerId)?.Name,
            EntryTime = a.AdmissionTime,
            FinishTime = a.ActualEndTime,
            AssignmentStatus = (BO.AssignmentStatus?)a.AssignmentStatus
        }).ToList();

        // Create an object of the logical entity type "Call"
        var boCall = new BO.Call
        {
            Id = doCall.Id,
            CallType = (BO.CallType)doCall.CallType,
            Description = doCall.Description,
            FullAddress = doCall.FullAddress,
            Latitude = doCall.Latitude,
            Longitude = doCall.Longitude,
            OpeningTime = doCall.OpeningTime,
            MaxCompletionTime = doCall.MaxCompletionTime,
            Status = doCall.Status,
            CallAssigns = callAssignments
        };

        return boCall;
    }

    public IEnumerable<BO.CallInList> GetFilteredCalls(BO.CallType? filterCallType, BO.AssignmentStatus? filterStatus, Enum? sortField)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.ClosedCallInList> GetVolunteerClosedCallsHistory(int volunteerId, BO.CallType? callTypeFilter, Enum? sortField)
    {
        throw new NotImplementedException();
    }

    public void MarkAssignmentAsCompleted(int volunteerId, int assignmentId)
    {
        // Fetch the assignment details from the data layer
        var assignment = _dal.Assignment.Read(assignmentId);
        if (assignment == null)
        {
            throw new BO.BoDoesNotExistException($"Assignment with ID={assignmentId} does not exist.");
        }

        // Check if the requester is the volunteer for whom the assignment is registered
        if (assignment.VolunteerId != volunteerId)
        {
            throw new BO.BoInvalidOperationException("Requester does not have permission to complete this assignment.");
        }

        // Check that the assignment is open and has not been processed or expired
        if (assignment.AssignmentStatus != DO.AssignmentStatus.Open || assignment.ActualEndTime != null)
        {
            throw new BO.BoInvalidOperationException("The assignment has already been processed or expired.");
        }

        try
        {
            _dal.Assignment.Update(new Assignment(assignment.Id, assignment.CallId, assignment.VolunteerId, assignment.AdmissionTime, ClockManager.Now, AssignmentStatus.Completed));
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BoDoesNotExistException($"Assignment with ID={assignmentId} does not exist.", ex);
        }
    }

    public void UpdateCall(BO.Call boCall)
    {
        double latitude,longitude;
        try
        {
            // Check the correctness of the values logically
            (latitude, longitude) = Helpers.Tools.GetCoordinates(boCall.FullAddress);
            boCall.Latitude = latitude;
            boCall.Longitude = longitude;
        }
        catch (Exception ex)
        {
            throw new BO.BoInvalidOperationException("Invalid address. Unable to retrieve coordinates.", ex);
        }
       
        if (boCall.MaxCompletionTime.HasValue && boCall.MaxCompletionTime.Value < boCall.OpeningTime)
        {
            throw new ArgumentException("Maximum completion time must be greater than the opening time.", nameof(boCall.MaxCompletionTime));
        }

        // Create an object of the data entity type DO.Call
        DO.Call doCall = new DO.Call(
            boCall.Id,
            (DO.CallType)boCall.CallType, // Convert BO.CallType to DO.CallType
            boCall.Description,
            boCall.FullAddress,
            latitude,
            longitude,
            boCall.OpeningTime,
            boCall.MaxCompletionTime
        );

        try
        {
            // Attempt to request an update of the call in the data layer
            _dal.Call.Update(doCall);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            // If there is no call with the ID number received as a parameter, throw a suitable exception
            throw new BO.BoDoesNotExistException($"Call with ID={boCall.Id} does not exist.", ex);
        }
    }
}
