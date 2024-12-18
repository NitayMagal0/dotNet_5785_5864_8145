namespace BO;

public class Call
{
    public int Id { get; init; }
    public CallType CallType { get; set; }
    public string? Description { get; set; } // Nullable string for additional call details
    public string? FullAddress { get; set; } // Full address of the call location
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime OpeningTime { get; set; } // Opening time of the call
    public DateTime? MaxCompletionTime { get; set; } // Nullable maximum completion time for the call
    public CallStatus Status { get; set; } // Nullable enum for the status of the assignment
    public List<BO.CallAssignInList>? CallAssigns { get; set; }

    public override string ToString()
    {
        var callAssignsString = CallAssigns != null ? string.Join(", ", CallAssigns.Select(ca => ca.ToString())) : "None";
        return $"Call ID: {Id}\n" +
               $"Call Type: {CallType}\n" +
               $"Description: {Description}\n" +
               $"Full Address: {FullAddress}\n" +
               $"Latitude: {Latitude}\n" +
               $"Longitude: {Longitude}\n" +
               $"Opening Time: {OpeningTime}\n" +
               $"Max Completion Time: {MaxCompletionTime}\n" +
               $"Status: {Status}\n" +
               $"Call Assigns: {callAssignsString}";
    }
}

