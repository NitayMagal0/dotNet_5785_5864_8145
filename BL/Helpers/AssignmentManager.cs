using DO;

namespace Helpers;

internal class AssignmentManager
{
    private static readonly DalApi.IDal _dal = DalApi.Factory.Get; //stage 4

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
        var assignments = _dal.Assignment.ReadAll().Where(a => a.ActualEndTime.HasValue && a.ActualEndTime.Value < newClock);

        foreach (var assignment in assignments)
        {
            var call = _dal.Call.Read(assignment.CallId);

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
                _dal.Assignment.Update(updatedAssignment);

               CallManager.PeriodicCallsUpdates(oldClock, newClock);
            }
        }
    }
}

