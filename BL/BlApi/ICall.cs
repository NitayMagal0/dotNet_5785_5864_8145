using BO;

namespace BlApi;

public interface ICall : IObservable
{
    int[] GetCallCountsByStatus();
    IEnumerable<CallInList> GetFilteredAndSortedCalls(CallType? filterField, CallStatus? filterValue, Enum? sortField);
    BO.Call GetCallDetails(int callId);
    void UpdateCall(BO.Call call);
    void DeleteCall(int callId);
    void AddCall(BO.Call call);
    IEnumerable<BO.ClosedCallInList> GetVolunteerClosedCallsHistory(int volunteerId, BO.CallType? callTypeFilter, Enum? sortField);
    IEnumerable<BO.OpenCallInList> GetAvailableOpenCallsForVolunteer(int volunteerId, BO.CallType? callTypeFilter, Enum? sortField);
    void MarkAssignmentAsCompleted(int volunteerId, int assignmentId);
    void CancelAssignment(int requesterId, int callId);
    void AssignCallToVolunteer(int volunteerId, int callId);
    public BO.Call GetCallsForVolunteer(int volunteerId);
    public CallInList GetCallInListById(int callId);
    public IEnumerable<OpenCallInList> GetNearbyOpenCallsForVolunteer(int volunteerId, double range, DistanceType distanceType, CallType? callTypeFilter, Enum? sortField);
}

