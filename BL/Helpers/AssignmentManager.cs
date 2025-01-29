using DO;

namespace Helpers;

internal class AssignmentManager
{
    private static readonly DalApi.IDal _dal = DalApi.Factory.Get; //stage 4
    internal static ObserverManager Observers = new(); //stage 5 
    public static BO.AssignmentStatus MapAssignmentStatus(DO.AssignmentStatus? status)
    {
        return status switch
        {
            DO.AssignmentStatus.Completed => BO.AssignmentStatus.Completed,
            DO.AssignmentStatus.CancelledByUser => BO.AssignmentStatus.CancelledByUser,
            DO.AssignmentStatus.CancelledByAdmin => BO.AssignmentStatus.CancelledByAdmin,
            DO.AssignmentStatus.ExpiredCancellation => BO.AssignmentStatus.ExpiredCancellation,
            _ => throw new ArgumentException("Invalid assignment status")
        };
    }

    internal static void PeriodicAssignmentsUpdates(DateTime oldClock, DateTime newClock)
    {
        IEnumerable<Assignment> assignments;
        lock (AdminManager.BlMutex)
             assignments = _dal.Assignment.ReadAll().Where(a => a.ActualEndTime.HasValue && a.ActualEndTime.Value < newClock);

        foreach (var assignment in assignments)
        {
            DO.Call call;
            lock (AdminManager.BlMutex)
                 call = _dal.Call.Read(assignment.CallId);

            if (call == null)
            {
                throw new ArgumentException($"Call with ID {assignment.CallId} does not exist.");
            }

            // Check if the assignment is still active and needs to be updated
            if (assignment.AssignmentStatus == null)
            {
                // Update the assignment with "ExpiredCancellation" status
                var updatedAssignment = new Assignment
                {
                    Id = assignment.Id,
                    CallId = assignment.CallId,
                    VolunteerId = assignment.VolunteerId,
                    AdmissionTime = assignment.AdmissionTime,
                    ActualEndTime = newClock,
                    AssignmentStatus = DO.AssignmentStatus.ExpiredCancellation
                };
                lock (AdminManager.BlMutex)
                    _dal.Assignment.Update(updatedAssignment);
                AssignmentManager.Observers.NotifyItemUpdated(updatedAssignment.Id);  //stage 5
                AssignmentManager.Observers.NotifyListUpdated();  //stage 5
                CallManager.PeriodicCallsUpdates(oldClock, newClock);
            }
        }
    }
}

