namespace BlImplementation;
using BlApi;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void AddCall(BO.Call call)
    {
        throw new NotImplementedException();
    }

    public void AssignCallToVolunteer(int volunteerId, int callId)
    {
        throw new NotImplementedException();
    }

    public void CancelAssignment(int requesterId, int assignmentId)
    {
        throw new NotImplementedException();
    }

    public void DeleteCall(int callId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.OpenCallInList> GetAvailableOpenCallsForVolunteer(int volunteerId, BO.CallType? callTypeFilter, Enum? sortField)
    {
        throw new NotImplementedException();
    }

    public int[] GetCallCountsByStatus()
    {
        throw new NotImplementedException();
    }

    public BO.Call GetCallDetails(int callId)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public void UpdateCall(BO.Call call)
    {
        throw new NotImplementedException();
    }
}
