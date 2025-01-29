using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using BlImplementation;
using DalApi;

namespace Helpers;

internal class VolunteerManager
{
    private static readonly DalApi.IDal _dal = DalApi.Factory.Get;
    internal static ObserverManager Observers = new(); //stage 5 

    private static readonly Random s_rand = new();
    private static int s_simulatorCounter = 0;


    internal static void SimulateVolunteerActivities()
    {
        var callManager = new BlImplementation.CallImplementation(); // we need this to use callimplementation functions
        Thread.CurrentThread.Name = $"Simulator{++s_simulatorCounter}";

        // Retrieve all active volunteers
        List<DO.Volunteer> activeVolunteers;
        lock (AdminManager.BlMutex)
        {
            activeVolunteers = _dal.Volunteer.ReadAll(v => v.IsActive).ToList();
        }

        foreach (var volunteer in activeVolunteers)
        {
            // Volunteer has a call in progress
            var currentCall = VolunteerManager.GetCurrentCallInProgress(volunteer.Id);

            // if the volunteer doesnt have a call in progress - 
            // 20% chance to assign a new call to the volunteer
            if (currentCall==null)
            {
                // Volunteer does not have a call in progress
                // 20% chance to assign a new call
                if (s_rand.Next(1, 101) <= 20)
                {
                    // Retrieve open calls, Converted to list so we can use the count function in selectCall
                    var openCalls = callManager.GetAvailableOpenCallsForVolunteer(volunteer.Id,null,null).ToList();

                    if (openCalls.Any())
                    {
                        // Randomly select a call for the volunteer
                        var selectedCall = openCalls[s_rand.Next(openCalls.Count())];
                        callManager.AssignCallToVolunteer(volunteer.Id, selectedCall.Id);
                    }
                }
            }
            // If the volunteer has a call in progress - 
            // If enough time has passed since the start of the treatment - close the call

            else
            {
                // Calculate the time since the call was opened
                TimeSpan timeSinceStart = AdminManager.Now - currentCall.OpeningTime;


                // Calculate the air distance between the volunteer and the call
                double airDistance = Helpers.Tools.CalculateAirDistance(
                 (volunteer.Latitude ?? 0.0, volunteer.Longitude ?? 0.0), // Starting point
                 (currentCall.Latitude ?? 0.0, currentCall.Longitude ?? 0.0) // Destination point
);

                // Calculate the estimated treatment time based on the air distance between the volunteer and the call location.
                // - The base treatment time is derived from the air distance (in kilometers), assuming 2 minutes of treatment/travel time per kilometer: TimeSpan.FromMinutes(airDistance * 2).
                // - A random additional time (between 5 and 14 minutes) is added to simulate variability in treatment or travel time: TimeSpan.FromMinutes(s_rand.Next(5, 15)).
                // - The total estimated treatment time is the sum of the base time and the random additional time.
                // For example, if the airDistance is 10 km and the random number is 7, the estimated treatment time will be: (10 * 2) + 7 = 27 minutes.
                TimeSpan estimatedTreatmentTime = TimeSpan.FromMinutes(airDistance * 2) + TimeSpan.FromMinutes(s_rand.Next(5, 15));

                if (timeSinceStart >= estimatedTreatmentTime)
                    {
                    // Mark the call as completed
                    
                    callManager.MarkAssignmentAsCompleted(volunteer.Id, currentCall.Id);

                    }
                    else
                    {
                        // 10% chance to cancel the current call
                        if (s_rand.Next(1, 101) <= 10)
                        {
                            callManager.CancelAssignment(volunteer.Id, currentCall.Id);
                        }
                    }

            }
        }
    }

    /// <summary>
    /// Checks if a volunteer has a call in progress.
    /// </summary>
    /// <param name="volunteerId"></param>
    /// <returns></returns>
    internal static bool DoesVolunteerHaveCallInProgress(int volunteerId)
        {
        DO.Assignment assignment;
        lock (AdminManager.BlMutex)
            assignment = _dal.Assignment.Read(a => a.VolunteerId == volunteerId &&
                                                  _dal.Call.Read(a.CallId).MaxCompletionTime > AdminManager.Now);
        return assignment != null;
    }

    internal static BO.Call GetCurrentCallInProgress(int volunteerId)
    {
        DO.Assignment assignment;
        lock (AdminManager.BlMutex)
            assignment = _dal.Assignment.Read(a => a.VolunteerId == volunteerId &&
                                                  _dal.Call.Read(a.CallId).MaxCompletionTime > AdminManager.Now);
        if (assignment == null)
            return null;
        DO.Call call;
        lock (AdminManager.BlMutex)
            call = _dal.Call.Read(assignment.CallId);
        return CallManager.ConvertCallToBO(call);
    }
    /// <summary>
    /// Convert BO.Volunteer to DO.Volunteer
    /// </summary>
    /// <param name="volunteer"></param>
    /// <returns></returns>
    internal static DO.Volunteer ConvertVolunteerToDO(BO.Volunteer volunteer)
    {
        return new DO.Volunteer
        {
            Id = volunteer.Id,
            FullName = volunteer.FullName,
            MobilePhone = volunteer.MobilePhone,
            Email = volunteer.Email,
            Password = volunteer.Password,
            FullAddress = volunteer.FullAddress,
            Latitude = volunteer.Latitude,
            Longitude = volunteer.Longitude,
            Role = MapRole(volunteer.Role),
            IsActive = volunteer.IsActive,
            MaxDistanceForCall = volunteer.MaxDistanceForCall,
            DistanceType = MapDistanceType(volunteer.DistanceType)
        };
    }

    /// <summary>
    /// Convert Enum Role from BO to DO
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal static DO.Role MapRole(BO.Role role)
    {
        return role switch
        {
            BO.Role.Manager => DO.Role.Manager,
            BO.Role.Volunteer => DO.Role.Volunteer,
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };
    }

    /// <summary>
    /// Convert Enum DistanceType from BO to DO
    /// </summary>
    /// <param name="distanceType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal static DO.DistanceType MapDistanceType(BO.DistanceType distanceType)
    {
        return distanceType switch
        {
            BO.DistanceType.AirDistance => DO.DistanceType.AirDistance,
            BO.DistanceType.WalkingDistance => DO.DistanceType.WalkingDistance,
            BO.DistanceType.DrivingDistance => DO.DistanceType.DrivingDistance,
            _ => throw new ArgumentOutOfRangeException(nameof(distanceType), distanceType, null)
        };
    }

    /// <summary>
    /// Convert DO.Volunteer to BO.Volunteer
    /// </summary>
    /// <param name="volunteer"></param>
    /// <returns></returns>
    /// <summary>
    /// Convert DO.Volunteer to BO.Volunteer
    /// </summary>
    /// <param name="volunteer"></param>
    /// <returns></returns>
    internal static BO.Volunteer ConvertVolunteerToBO(DO.Volunteer volunteer)
    {
        if (volunteer == null)
            throw new BO.BlNullReferenceException("volunteer can't be null");

        // Retrieve all assignments for the volunteer at once
        IEnumerable<DO.Assignment> assignments;
        lock (AdminManager.BlMutex)
             assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteer.Id);

        // Calculate handled, canceled, and expired calls in memory
        var handledCallsCount = assignments.Count(a => a.AssignmentStatus.HasValue &&
                                                       a.AssignmentStatus.Value == DO.AssignmentStatus.Completed);
        var canceledCallsCount = assignments.Count(a => a.AssignmentStatus.HasValue &&
                                                        (a.AssignmentStatus.Value == DO.AssignmentStatus.CancelledByUser ||
                                                         a.AssignmentStatus.Value == DO.AssignmentStatus.CancelledByAdmin));
        var expiredCallsCount = assignments.Count(a => a.AssignmentStatus.HasValue &&
                                                       a.AssignmentStatus.Value == DO.AssignmentStatus.ExpiredCancellation);

        // Convert and return the BO.Volunteer object
        return new BO.Volunteer
        {
            Id = volunteer.Id,
            FullName = volunteer.FullName,
            MobilePhone = volunteer.MobilePhone,
            Email = volunteer.Email,
            Password = volunteer.Password,
            FullAddress = volunteer.FullAddress,
            Latitude = volunteer.Latitude,
            Longitude = volunteer.Longitude,
            Role = MapRole(volunteer.Role),
            IsActive = volunteer.IsActive,
            MaxDistanceForCall = volunteer.MaxDistanceForCall,
            DistanceType = MapDistanceType(volunteer.DistanceType),
            CallInProgress = GetCallInProgress(volunteer.Id),
            HandledCalls = handledCallsCount,
            CanceledCalls = canceledCallsCount,
            ExpiredCalls = expiredCallsCount
        };
    }

    /// <summary>
    /// Convert Enum Role from DO to BO
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal static BO.Role MapRole(DO.Role role)
    {
        return role switch
        {
            DO.Role.Manager => BO.Role.Manager,
            DO.Role.Volunteer => BO.Role.Volunteer,
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };
    }

    /// <summary>
    /// Convert Enum DistanceType from DO to BO
    /// </summary>
    /// <param name="distanceType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    internal static BO.DistanceType MapDistanceType(DO.DistanceType distanceType)
    {
        return distanceType switch
        {
            DO.DistanceType.AirDistance => BO.DistanceType.AirDistance,
            DO.DistanceType.WalkingDistance => BO.DistanceType.WalkingDistance,
            DO.DistanceType.DrivingDistance => BO.DistanceType.DrivingDistance,
            _ => throw new ArgumentOutOfRangeException(nameof(distanceType), distanceType, null)
        };
    }

    /// <summary>
    /// Retrieves the details of a volunteer by their ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer.</param>
    /// <returns>A BO.Volunteer object containing the volunteer's details.</returns>
    /// <exception cref="Exception">Thrown when the volunteer does not exist.</exception>
    internal BO.Volunteer GetVolunteerDetails(int id)
    {
        DO.Volunteer volunteer;
        lock (AdminManager.BlMutex)
             volunteer = _dal.Volunteer.Read(id);
        if (volunteer == null)
            throw new Exception("Volunteer doesn't exist");//Retrive without the call in progress
        return ConvertVolunteerToBO(volunteer);
    }
    /// <summary>
    /// Validates if the given volunteer object is valid.
    /// </summary>
    /// <param name="volunteer">The volunteer to validate</param>
    /// <returns>True - the volunteer values are valid, false otherwise</returns>
    internal static bool IsValidVolunteer(BO.Volunteer volunteer)
    {
        try
        {
            // Check if the address is valid
            Tools.GetCoordinates(volunteer.FullAddress);
        }
        catch (Exception)
        {
            throw new Exception("Address isn't valid");
        }

        // Check if the email is valid
        if (!IsValidEmail(volunteer.Email))
        {
            throw new Exception("Email isn't valid");
        }

        // Check if the phone number is valid
        if (!IsValidPhoneNumber(volunteer.MobilePhone))
        {
            throw new Exception("Mobile phone number isn't valid");
        }

        // Check if the ID is valid
        if (!IsValidID(volunteer.Id))
        {
            throw new Exception("ID isn't valid");
        }

        // Check if the password is strong
        if (!IsStrongPassword(volunteer.Password))
        {
            throw new Exception("Password isn't strong enough");
        }

        // If all checks pass
        return true;
    }

    /// <summary>
    /// Validates if the given string is a valid email.
    /// </summary>
    /// <param name="email">The string to validate</param>
    /// <returns>True - the string is a valid email address, false otherwise</returns>
    internal static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        // Regular expression to validate email format
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        return Regex.IsMatch(email, emailPattern);
    }

    /// <summary>
    /// Validates if the given string is a valid phone number.
    /// </summary>
    /// <param name="phone">The string to validate</param>
    /// <returns>True if the phone number is valid, false otherwise.</returns>
    internal static bool IsValidPhoneNumber(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        // Regular expression to validate phone number format
        string phonePattern = @"^0\d([\d]{0,1})([-]{0,1})\d{7}$";

        return Regex.IsMatch(phone, phonePattern);
    }

    /// <summary>
    /// Validates if the given ID is a valid Israeli ID.
    /// </summary>
    /// <param name="id">The ID to validate.</param>
    /// <returns>True if the ID is valid, false otherwise.</returns>
    internal static bool IsValidID(int id)
    {
        string idString = id.ToString("D9"); // Ensure the ID has 9 digits
        if (idString.Length != 9)
            return false;

        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            int digit = idString[i] - '0';
            int multiplied = digit * ((i % 2) + 1);
            sum += (multiplied > 9) ? multiplied - 9 : multiplied;
        }

        return sum % 10 == 0;
    }

    internal static bool IsStrongPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            return false;

        // Check length
        if (password.Length < 8)
            return false;

        // Check for at least one uppercase letter
        if (!password.Any(char.IsUpper))
            return false;

        // Check for at least one lowercase letter
        if (!password.Any(char.IsLower))
            return false;

        // Check for at least one digit
        if (!password.Any(char.IsDigit))
            return false;

        // Check for whitespace
        if (password.Any(char.IsWhiteSpace))
            return false;

        return true;
    }

    /// <summary>
    /// Retrieves the call in progress for a volunteer by their ID.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <returns>A BO.CallInProgress object containing the call in progress details.</returns>
    /// <exception cref="Exception">Thrown when the volunteer does not have a call in progress.</exception>
    internal static BO.CallInProgress GetCallInProgress(int volunteerId)
    {
        // Retrieve the volunteer and related assignment in one go
        DO.Volunteer volunteer;
        lock (AdminManager.BlMutex)
             volunteer = _dal.Volunteer.Read(volunteerId);
        if (volunteer == null)
            throw new Exception("Volunteer not found");

        // Retrieve the assignment for the volunteer
        DO.Assignment assignment;
        lock (AdminManager.BlMutex)
             assignment = _dal.Assignment.Read(a => a.VolunteerId == volunteerId &&
                                                   _dal.Call.Read(a.CallId).MaxCompletionTime > AdminManager.Now);

        if (assignment == null)
            return null;

        try
        {
            // Retrieve the call details once
            DO.Call call;
            lock (AdminManager.BlMutex)
                 call = _dal.Call.Read(assignment.CallId);

            // Prepare the distance type
            BO.DistanceType distanceType = MapDistanceType(volunteer.DistanceType);

            // Calculate distance
            var distanceFromVolunteer = Tools.GetResultAsync(
                ((double)volunteer.Latitude, (double)volunteer.Longitude),
                (call.Latitude, call.Longitude), distanceType);

            // Determine call status
            var callStatus = CallManager.IsCallInRiskRange(call.Id)
                ? BO.CallStatus.OpenAtRisk
                : BO.CallStatus.InProgress;

            // Return the CallInProgress object
            return new BO.CallInProgress
            {
                Id = assignment.Id,
                CallId = assignment.CallId,
                CallType = CallManager.MapCallType(call.CallType),
                Description = call.Description,
                FullAddress = call.FullAddress,
                OpeningTime = call.OpeningTime,
                MaxCompletionTime = call.MaxCompletionTime,
                AdmissionTime = assignment.AdmissionTime,
                DistanceFromVolunteer = distanceFromVolunteer,
                Status = callStatus
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error creating CallInProgress: {ex.Message}");
        }
    }

    /// <summary>
    /// Encodes the given password to a Base64 string.
    /// </summary>
    /// <param name="password">The password to encode.</param>
    /// <returns>The Base64 encoded string of the password.</returns>
    public static string EncodePassword(string password)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(password);
        return Convert.ToBase64String(plainTextBytes);
    }

    /// <summary>
    /// Decodes the given Base64 encoded password.
    /// </summary>
    /// <param name="encodedPassword">The Base64 encoded password to decode.</param>
    /// <returns>The decoded password as a string.</returns>
    public static string DecodePassword(string encodedPassword)
    {
        var base64EncodedBytes = Convert.FromBase64String(encodedPassword);
        return Encoding.UTF8.GetString(base64EncodedBytes);
    }
    
}

