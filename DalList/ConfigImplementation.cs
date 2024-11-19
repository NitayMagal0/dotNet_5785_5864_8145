using DalApi;
namespace Dal;


/// <summary>
/// The implementation of the config interface
/// </summary>

public class ConfigImplementation : IConfig
{
    /// <summary>
    /// These are Get functions for the next running ID number
    /// </summary>
    public int NextCallId
    {
        get => Config.NextCallId;
    }

    public int NextAssignmentId
    {
        get { return Config.NextAssignmentId; }
    }


    /// <summary>
    /// Functions that are responsible for returning the variables related to time
    /// </summary>
    public TimeSpan RiskRange
    {
        get => Config.RiskRange;
        set => Config.RiskRange = value;
    }

    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }


    //initialization function
    public void Reset()
    {
        Config.Reset();
    }
}