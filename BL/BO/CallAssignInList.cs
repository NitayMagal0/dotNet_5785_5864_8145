namespace BO;

public class CallAssignInList
{
    public int? VolunteerId { get; set; }
    public string? VolunteerName { get; set; }
    public DateTime EntryTime { get; set; }  //The time the volunteer entered the call 
    public DateTime? FinishTime { get; set; }
    public AssignmentStatus? AssignmentStatus { get; set; }

    public override string ToString()
    {
        return "-----------------------------------" + "\n" +
               $"VolunteerId: {VolunteerId}\n" +
               $"VolunteerName: {VolunteerName}\n" +
               $"EntryTime: {EntryTime}\n" +
               $"FinishTime: {FinishTime}\n" +
               $"AssignmentStatus: {AssignmentStatus}\n";
    }
}

