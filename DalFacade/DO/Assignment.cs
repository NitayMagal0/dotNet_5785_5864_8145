namespace DO;
/// <summary>
/// Represents an assignment given to a volunteer, tracking the details of the assignment timing, associated call, and treatment outcome.
/// </summary>
/// <param name="Id">Unique identifier for the assignment.</param>
/// <param name="CallId">Identifier of the associated call that this assignment is related to.</param>
/// <param name="VolunteerId">Identifier of the volunteer assigned to this call.</param>
/// <param name="AdmissionTime">The date and time the volunteer was assigned to the call. Defaults to the current date and time.</param>
/// <param name="ActualEndTime">The actual date and time when the assignment ended. Can be null if the assignment is still ongoing.</param>
/// <param name="AssignmentStatus">The type of treatment outcome that marked the end of the assignment.</param>
public record Assignment
(
    int Id,
    int CallId,
    int VolunteerId,
    DateTime AdmissionTime = default,
    DateTime? ActualEndTime = null,
    AssignmentStatus? AssignmentStatus = null
)
{
    /// <summary>
    /// A default constructor initializes with default values.
    /// </summary>
    public Assignment() : this(0, 0, 0, DateTime.Now) { }
}
