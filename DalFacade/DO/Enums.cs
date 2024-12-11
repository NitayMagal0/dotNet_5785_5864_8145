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
    Undefined,
    CleaningShelters,
    HelpForFamiliesInNeed,
    FoodPackagingForNeedyFamilies,
    HospitalVisitsForMoraleBoost
}


/// <summary>
/// enum for the treatment termination type
/// </summary>
//public enum TreatmentEndType
//{
//    Completed,
//    CancelledByUser,
//    CancelledByAdmin,
//    Expired
//}
public enum AssignmentStatus
{
    Undefined,
    Open,
    InProgress,
    Completed,
    CancelledByUser,
    CancelledByAdmin,
    OpenAtRisk,
    InProgressAtRisk
}
public enum MenuOptions
{
    Exit = 0,
    VolunteerSubMenu = 1,
    CallSubMenu = 2,
    AssignmentSubMenu = 3,
    Initialize = 5,
    ShowAll = 6,
    ConfigSubMenu = 7,
    Reset = 8
}
public enum SubMenuOptions
{
    Exit = 0,
    AddObject = 1,
    ShowObject = 2,
    ShowList = 3,
    Update = 4,
    DeleteObject = 5,
    DeleteAll = 6
}

public enum ConfigSumMenuOptions
{
    Exit = 0,
    AddMinuteToClock = 1,
    AddHourToClock = 2,
    AddDayToClock = 3,
    ShowCurrentClockValue = 4,
    SetCurrentValue = 5,
    ShowCurrentValue = 6,
    Reset = 7

}

public enum ConfigOptions
{
    Clock = 1,
    RiskRange = 2
}
