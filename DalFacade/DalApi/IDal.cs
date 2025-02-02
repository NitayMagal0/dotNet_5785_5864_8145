namespace DalApi;

/// <summary>
/// Interface for Data Access Layer (DAL) operations.
/// </summary>
public interface IDal
{
    /// <summary>
    /// Gets the assignment operations.
    /// </summary>
    IAssignment Assignment { get; }

    /// <summary>
    /// Gets the call operations.
    /// </summary>
    ICall Call { get; }

    /// <summary>
    /// Gets the configuration operations.
    /// </summary>
    IConfig Config { get; }

    /// <summary>
    /// Gets the volunteer operations.
    /// </summary>
    IVolunteer Volunteer { get; }

    /// <summary>
    /// Resets the database to its initial state.
    /// </summary>
    void ResetDB();
}

