namespace BO;

public class ClosedCallInList
{
    public int Id { get; init; }
    public CallType CallType { get; set; }
    public string FullAddress { get; set; }
    public DateTime OpeningTime { get; set; } // Opening time of the call
    public DateTime EntryTime { get; set; }  //The time the volunteer entered the call 
    public DateTime? FinishTime { get; set; }
    public AssignmentStatus? AssignmentStatus { get; set; }
}


