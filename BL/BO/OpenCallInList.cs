namespace BO;

public class OpenCallInList
{
    public int Id { get; init; }
    public CallType CallType { get; init; }
    public string? CallTypeDescription { get; set; }
    public string FullAddress { get; init; }
    public DateTime OpeningTime { get; set; } // Opening time of the call
    public DateTime? MaxCompletionTime { get; set; } // Nullable maximum completion time for the call
    public double DistanceFromVolunteer { get; set; }
}


