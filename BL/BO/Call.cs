namespace BO;

public class Call
{
    public int Id { get; init; }
    public CallType CallType { get; init; }
    public string? Description { get; set; } // Nullable string for additional call details
    public string? FullAddress { get; set; } // Full address of the call location
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime OpeningTime { get; set; } // Opening time of the call
    public DateTime? MaxCompletionTime { get; set; } // Nullable maximum completion time for the call
    public AssignmentStatus Status { get; set; } // Nullable enum for the status of the assignment
    public List<BO.CallAssignInList>? CallAssigns { get; set; }
}

