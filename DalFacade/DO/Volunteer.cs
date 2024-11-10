namespace DO;
/// <summary>
/// The volunteer entity contains personal details for a "volunteer", including a unique ID number.
/// </summary>
/// <param name="Id">ID number of the volunteer</param>
/// <param name="FullName">The name of the volunteer</param>
/// <param name="MobilePhone">Volunteer's phone number</param>
/// <param name="Email">The email of the volunteer</param>
/// <param name="Password">The user's password for the system</param>  An addition should be decided whether to do!!!
/// <param name="FullAddress">The address of the volunteer</param>
/// <param name="Latitude">Latitude is calculated by the logic layer</param>
/// <param name="Longitude">Longitude is calculated by the logic layer</param>
/// <param name="Role">The role of the volunteer is a manager or volunteer</param>
/// <param name="IsActive">Is the volunteer still active in the organization or retired</param>
/// <param name="MaxDistanceForCall">The maximum reading distance for the volunteer</param>
/// <param name="DistanceType">The type of distance calculation preferred by the user</param>   Addendum you have to choose whether to do!!!!
public record Volunteer
(
    int Id,
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
    DistanceType DistanceType = DistanceType.AirDistance
)
{
    /// <summary>
    /// A default constructor initializes with default values.
    /// </summary>
    public Volunteer() : this(0, "", "", "") { }
}
