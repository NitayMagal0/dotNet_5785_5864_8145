using BlApi;
namespace BlImplementation;
/// <summary>
/// Implementation of the BL (Business Logic) layer, providing access to Volunteer, Call, and Admin operations.
/// </summary>
internal class Bl : IBl
{
    /// <summary>
    /// Gets the implementation of the Admin operations interface.
    /// </summary>
    public IAdmin Admin { get; } = new AdminImplementation();

    /// <summary>
    /// Gets the implementation of the Call operations interface.
    /// </summary>
    public ICall Call { get; } = new CallImplementation();

    /// <summary>
    /// Gets the implementation of the Volunteer operations interface.
    /// </summary>
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();
}

