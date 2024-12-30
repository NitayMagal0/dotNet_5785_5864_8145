namespace BO;

/// <summary>
/// Enum to define the status of the assignment.
/// </summary>
public enum AssignmentStatus
{
    Completed,
    CancelledByUser,
    CancelledByAdmin,
    ExpiredCancellation
}

/// <summary>
/// Enum to define the status of the call.
/// </summary>
public enum CallStatus
{
    Open,
    InProgress,
    Completed,
    Expired,
    OpenAtRisk,
    InProgressAtRisk
}

/// <summary>
/// enum for the call type
/// </summary>
public enum CallType
{
    Undefined,
    CleaningShelters,
    HelpForFamiliesInNeed,
    FoodPackagingForNeedyFamilies,
    HospitalVisitsForMoraleBoost
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
public enum TimeUnit
{
    second,
    minute,
    hour,
    day,
    month,
    year
}
