namespace DalApi;

public interface IConfig
{
    int NextCallId { get; } //Gets the next CallId.

    int NextAssignmentId { get; } //Gets the next AssignmentId.

    DateTime Clock { get; set; } //Gets or sets the current system time.

    TimeSpan RiskRange { get; set; } //Gets or sets the risk range for the current assignment.

    void Reset(); //Resets the configuration to its default state.
}
