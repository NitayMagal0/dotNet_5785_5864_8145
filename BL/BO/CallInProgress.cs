using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    /// <summary>
    /// Represents a call in progress, containing details about the type, description, location, timing, and status of the call.
    /// </summary>
    public class CallInProgress
    {
        public int Id { get; init; } // Unique identifier, immutable after initialization
        public int CallId { get; init; }

        public CallType CallType { get; set; } // Enum for the type of call
        public string? Description { get; set; } // Nullable string for additional call details
        public string? FullAddress { get; set; } // Full address of the call location
        public DateTime OpeningTime { get; set; } // Opening time of the call
        public DateTime? MaxCompletionTime { get; set; } // Nullable maximum completion time for the call
        public DateTime EntryTime { get; set; }  //The time the volunteer entered the call 
        public double? DistanceFromVolunteer { get; set; } // Nullable distance from the volunteer
        public AssignmentStatus? Status { get; set; } // Nullable enum for the status of the assignment
    }
}
