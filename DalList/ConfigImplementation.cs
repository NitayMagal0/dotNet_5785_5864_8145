using DalApi;

namespace Dal;

public class ConfigImplementation : IConfig
{

    public int NextCallId
    { 
        get => Config.NextCallId;
    }

    public int NextAssignmentId 
    { 
        get { return Config.NextAssignmentId; }
    }

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
    public void Reset()
    {
        Config.Reset();
    }
}