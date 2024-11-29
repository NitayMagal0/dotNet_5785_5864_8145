using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal
{
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
        internal static int NextCallId
        {
            get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCallId");
            private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallId", value);
        }

        internal static int NextAssignmentId
        {
            get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextAssignmentId");
            set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", value);
        }

        // Clock property
        internal static DateTime Clock
        {
            get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
            set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
        }

        // RiskRange property
        internal static TimeSpan RiskRange
        {
            get => XMLTools.GetConfigTimeSpanVal(s_data_config_xml, "RiskRange");
            set => XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "RiskRange", value);
        }

        // Reset method to reset values to defaults
        internal static void Reset()
        {
            XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", DateTime.Now);
            XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "RiskRange", TimeSpan.Zero);
            XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallId", startCallId);
            XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", startAssignmentId);
        }
    }
}