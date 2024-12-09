namespace BO;

public class Volunteer
{
    public int Id { get; init; }
    public string? FullName { get; set; }
    public string? MobilePhone { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? FullAddress { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    Role Role { get; set; }
    bool IsActive { get; set; }
    double? MaxDistanceForCall { get; set; }
    DistanceType DistanceType { get; set; }
    int HandledCalls { get; set; }
    int CanceledCalls { get; set; }
    int ExpiredCalls { get; set; }
    BO.CallInProgress? CallInProgress { get; set; }
}

