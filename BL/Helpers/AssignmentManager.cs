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

}

