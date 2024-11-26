using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal
{
    internal static class Config
    {
        internal const string s_data_config_xml = "data-config.xml";
        internal const string s_volunteers_xml = "volunteers.xml";
        internal const string s_calls_xml = "calls.xml";
        internal const string s_assignments_xml = "assignments.xml";

        internal static DateTime Clock
        {
            get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
            set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
        }

        internal static int NextCallId
        {
            get => XMLTools.GetAndIncreaseConfigIntVal(s_calls_xml, "NextCallId");
            private set =>XMLTools.SetConfigIntVal(s_calls_xml, "NextCallId", value);
        }



        internal static void Reset()
        {
            nextCallId = startCallId;
            nextAssignmentId = startAssignmentId;
            Clock = DateTime.Now;
            RiskRange = TimeSpan.Zero;
        }
    }
}
