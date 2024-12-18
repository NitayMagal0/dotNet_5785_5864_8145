namespace BO;

public class CallInList
{
    public int? Id { get; init; } // Unique identifier, immutable after initialization
    public int CallId { get; init; }
    public CallType CallType { get; set; } // Enum for the type of call
    public DateTime OpeningTime { get; set; } // Opening time of the call
    public TimeSpan? RemainingTime { get; set; } // Nullable remaining time for the call
    public string? LastVolunteer { get; set; } // Nullable string for the last volunteer to enter the call
    public TimeSpan? TotalTime { get; set; }
    public CallStatus? Status { get; set; } // Nullable enum for the status of the assignment
    public int AssignmentsCount { get; set; }

}

