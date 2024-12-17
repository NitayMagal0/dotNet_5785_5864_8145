namespace BO;

public class Volunteer
{
    public int Id { get; set; }
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

    public override string ToString()
    {
        return $"Id: {Id}\n" +
            $"FullName: {FullName}\n" +
            $"MobilePhone: {MobilePhone}\n" +
            $"Email: {Email}\n" +
            $"FullAddress: {FullAddress}\n" +
            $"Latitude: {Latitude}\n" +
            $"Longitude: {Longitude}\n" +
            $"Role: {Role}\n" +
            $"IsActive: {IsActive}\n" +
            $"MaxDistanceForCall: {MaxDistanceForCall}\n" +
            $"DistanceType: {DistanceType}\n" +
            $"HandledCalls: {HandledCalls}\n" +
            $"CanceledCalls: {CanceledCalls}\n" +
            $"ExpiredCalls: {ExpiredCalls}\n" +
            $"CallInProgress: {CallInProgress}";

    }
}


