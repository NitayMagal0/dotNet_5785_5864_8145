namespace DO;

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


/// <summary>
/// enum for the call type
/// </summary>
public enum CallType
{
    Undefined
    //According to the type of specific system (food preparation, food transportation, etc.) 
}


/// <summary>
/// enum for the treatment termination type
/// </summary>
public enum TreatmentEndType
{
    Completed,
    CancelledByUser,
    CancelledByAdmin,
    Expired
}