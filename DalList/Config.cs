namespace Dal;
/// <summary>
/// The config class takes care of the runner identification number of the Call and Assignment classes, it takes care 
/// of updating them and moving them by 1 when a new task is assigned
/// </summary>
internal static class Config
{
    internal const int startCallId = 1000;                      //The initial ID number of a call
    private static int nextCallId = startCallId;                //The running ID number of a call
    /// <summary>
    /// A get function that returns the next running ID number
    /// </summary>
    internal static int NextCallId { get => nextCallId++; }

    internal const int startAssignmentId = 1000;                //The initial ID number of a Assignment
    private static int nextAssignmentId = startAssignmentId;       //The running ID number of a Assignment
    /// <summary>
    /// A get function that returns the next running ID number
    /// </summary>
    internal static int NextAssignmentId { get => nextAssignmentId++; }

    /// <summary>
    /// The class is also responsible for the system clock
    /// </summary>
    internal static DateTime Clock { get; set; } = DateTime.Now;

    internal static TimeSpan RiskRange { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// An initialization function that resets all data back to the baseline values
    /// </summary>
    internal static void Reset()
    {
        nextCallId = startCallId;
        nextAssignmentId = startAssignmentId;
        Clock = DateTime.Now;
        RiskRange = TimeSpan.Zero;
    }
}
