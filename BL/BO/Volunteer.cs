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
    public Role Role { get; set; }
    public bool IsActive { get; set; }
    public double? MaxDistanceForCall { get; set; }
    public DistanceType DistanceType { get; set; }
    public int HandledCalls { get; set; }
    public int CanceledCalls { get; set; }
    public int ExpiredCalls { get; set; }
    public BO.CallInProgress? CallInProgress { get; set; }
}
/*int Id,
    string FullName,
    string MobilePhone,
    string Email,
    string? Password = null,
    string? FullAddress = null,
    double? Latitude = null,
    double? Longitude = null,
    Role Role = Role.Volunteer,
    bool IsActive = false,
    double? MaxDistanceForCall = null,
    DistanceType DistanceType = DistanceType.AirDistance*/

