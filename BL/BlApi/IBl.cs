namespace BlApi;
/// <summary>
/// Interface for the BL (Business Logic) layer, providing access to Volunteer, Call, and Admin operations.
/// </summary>
public interface IBl
{
    /// <summary>
    /// Gets the Volunteer operations interface.
    /// </summary>
    IVolunteer Volunteer { get; }

    /// <summary>
    /// Gets the Call operations interface.
    /// </summary>
    ICall Call { get; }

    /// <summary>
    /// Gets the Admin operations interface.
    /// </summary>
    IAdmin Admin { get; }
}

