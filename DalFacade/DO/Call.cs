namespace DO;
/// <summary>
/// The call entity contains details for a "call", including a unique runner ID number.
/// </summary>
/// <param name="Id">ID number of the call</param>
/// <param name="CallType">Type of call (food packaging, volunteering with children from special education, etc.)</param>
/// <param name="Description">Description of the call</param>
/// <param name="FullAddress">The location of the call</param>
/// <param name="Latitude">The call latitude is calculated by the logic layer</param>
/// <param name="Longitude">Longitude of the call is calculated by the logic layer</param>
/// <param name="OpeningTime">The opening time of the call by the manager</param>
/// <param name="MaxCompletionTime">Maximum time to finish call</param>
public record Call
(
    int Id,
    CallType CallType = CallType.Undefined,
    string? Description = null,
    string FullAddress = "",
    double Latitude = 0.0,
    double Longitude = 0.0,
    DateTime OpeningTime = default,
    DateTime? MaxCompletionTime = null
)
{
    /// <summary>
    /// A default constructor initializes with default values.
    /// </summary>
    public Call() : this(0) { }
}