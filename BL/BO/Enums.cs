namespace BO;

/// <summary>
/// Enum to define the status of the assignment.
/// </summary>
public enum AssignmentStatus
{
    Undefined,
    InProgress,
    Completed,
    Canceled
}

/// <summary>
/// Enum to define the type of call.
/// </summary>
public enum CallType
{
    Undefined,
    FoodPackaging,
    VolunteeringWithChildren,
    // Add additional call types as needed
}

/// <summary>
/// enum for the roles in the system
/// </summary>
public enum Role
{
    Manager,
    Volunteer
}
/// <summary>
/// enum for user preference of distance calculation
/// </summary>
public enum DistanceType
{
    AirDistance,
    WalkingDistance,
    DrivingDistance
}