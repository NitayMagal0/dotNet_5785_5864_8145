namespace DalApi;

public interface IConfig  
{
    internal const int startCallId = 1000;
    private static int nextCallId = startCallId;
    internal static int NextCallId { get => nextCallId++; }

    internal const int startAssignmentId = 1000;
    private static int nextAssignmentId = startAssignmentId;
    internal static int NextAssignmentId { get => nextAssignmentId++; }

    internal static DateTime Clock { get; set; } = DateTime.Now;

    internal static TimeSpan RiskRange { get; set; } = TimeSpan.Zero;

    //DateTime Clock { get; set; }
    //...
    void Reset();
}
