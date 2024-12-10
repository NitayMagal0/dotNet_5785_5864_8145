namespace BO;

public class VolunteerInList
{
    public int Id { get; init; }
    public string? FullName { get; set; }
    public bool IsActive { get; set; }
    public int HandledCalls { get; set; }
    public int CanceledCalls { get; set; }
    public int ExpiredCalls { get; set; }
    public int CallsInProgress { get; set; }
    public int? CallId { get; set; }
    public CallType CallType { get; set; }
}


