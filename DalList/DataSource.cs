namespace Dal;


/// <summary>
/// A static class that contains the database (lists) of the data layer
/// </summary>
internal static class DataSource
{
    internal static List<DO.Volunteer> Volunteers { get; } = new();
    internal static List<DO.Call> Calls { get; } = new();
    internal static List<DO.Assignment> Assignments { get; } = new();
}
