using DalApi;
namespace Dal;

/// <summary>
/// Singleton class for Data Access Layer (DAL) operations using lists.
/// </summary>
sealed internal class DalList : IDal
{
    // Singleton instance of DalList
    public static IDal Instance { get; } = new DalList();

    // Private constructor to prevent instantiation
    private DalList() { }

    // Implementation of IAssignment interface
    public IAssignment Assignment { get; } = new AssignmentImplementation();

    // Implementation of ICall interface
    public ICall Call { get; } = new CallImplementation();

    // Implementation of IConfig interface
    public IConfig Config { get; } = new ConfigImplementation();

    // Implementation of IVolunteer interface
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();

    /// <summary>
    /// Resets the database to its initial state by deleting all data.
    /// </summary>
    public void ResetDB()
    {
        Assignment.DeleteAll();
        Call.DeleteAll();
        Volunteer.DeleteAll();
        Config.Reset();
    }
}

