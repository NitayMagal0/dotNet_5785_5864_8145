using System.Runtime.CompilerServices;


namespace Dal;
/// <summary>
/// Configuration class for managing XML file names, default IDs, and other settings.
/// </summary>
internal static class Config
{
    // XML file names
    internal const string s_data_config_xml = "data-config.xml";
    internal const string s_calls_xml = "calls.xml";
    internal const string s_assignments_xml = "assignments.xml";
    internal const string s_volunteers_xml = "volunteers.xml";

    // Default starting IDs
    internal const int startCallId = 1000;
    internal const int startAssignmentId = 1000;

    // Properties for next ID
    /// <summary>
    /// Gets and increments the next available Call ID.
    /// </summary>
    internal static int NextCallId
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCallId");
        [MethodImpl(MethodImplOptions.Synchronized)]
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallId", value);
    }

    /// <summary>
    /// Gets and increments the next available Assignment ID.
    /// </summary>
    internal static int NextAssignmentId
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextAssignmentId");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", value);
    }

    // Clock property
    /// <summary>
    /// Gets or sets the current clock value.
    /// </summary>
    internal static DateTime Clock
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }

    // RiskRange property
    /// <summary>
    /// Gets or sets the risk range time span.
    /// </summary>
    internal static TimeSpan RiskRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => XMLTools.GetConfigTimeSpanVal(s_data_config_xml, "RiskRange");
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "RiskRange", value);
    }

    // Reset method to reset values to defaults
    /// <summary>
    /// Resets configuration values to their default settings.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    internal static void Reset()
    {
        XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", DateTime.Now);
        XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "RiskRange", TimeSpan.Zero);
        XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallId", startCallId);
        XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", startAssignmentId);
    }
}
