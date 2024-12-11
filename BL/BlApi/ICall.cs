using BO;

namespace BlApi;

    public interface ICall
{
    int[] GetCallCountsByStatus();
    IEnumerable<CallInList> GetFilteredAndSortedCalls(Enum? filterField, object? filterValue, Enum? sortField);
    BO.Call GetCallDetails(int callId);
    void UpdateCall(BO.Call call);
    void DeleteCall(int callId);
    void AddCall(BO.Call call);
    IEnumerable<BO.ClosedCallInList> GetVolunteerClosedCallsHistory(int volunteerId, BO.CallType? callTypeFilter, Enum? sortField);
    IEnumerable<BO.OpenCallInList> GetAvailableOpenCallsForVolunteer(int volunteerId, BO.CallType? callTypeFilter, Enum? sortField);
    void MarkAssignmentAsCompleted(int volunteerId, int assignmentId);
    void CancelAssignment(int requesterId, int assignmentId);
    void AssignCallToVolunteer(int volunteerId, int callId);
}

