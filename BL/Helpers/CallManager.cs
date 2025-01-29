using BlApi;
using BlImplementation;
using BO;
using DO;

namespace Helpers;

internal class CallManager
{
    internal static ObserverManager Observers = new(); //stage 5 
    private static readonly DalApi.IDal _dal = DalApi.Factory.Get; //stage 4

    internal static BO.CallStatus GetCallStatus(int callId)
    {
        // Fetch the call and its related data
        DO.Call call;
        IEnumerable<Assignment> assignments;
        DateTime systemClock;
        lock (AdminManager.BlMutex)
        {
             call = _dal.Call.Read(callId);
             assignments = _dal.Assignment.ReadAll().Where(a => a.CallId == callId);
             systemClock = AdminManager.Now;
        }
        if (call == null)
        {
            throw new ArgumentException("Invalid call ID");
        }

        // Handle case where no assignments exist
        if (!assignments.Any())
        {                                                                                            //It was AdminImplementation.GetMaxRange()
            if (call.MaxCompletionTime.HasValue && (call.MaxCompletionTime.Value - systemClock).TotalDays <= (_dal.Config.RiskRange).TotalDays)
            {
                return BO.CallStatus.OpenAtRisk;
            }
            return BO.CallStatus.Open;
        }

        // Determine status based on assignment states
        if (assignments.Any(a => a.AssignmentStatus.HasValue && a.AssignmentStatus.Value.ToString() == "Completed"))
        {
            return BO.CallStatus.Completed;
        }

        if (assignments.Any(a => a.AssignmentStatus.HasValue &&
                                 (a.AssignmentStatus.Value.ToString() == "CancelledByUser" ||
                                  a.AssignmentStatus.Value.ToString() == "CancelledByAdmin" ||
                                  a.AssignmentStatus.Value.ToString() == "ExpiredCancellation")))
        {
            return BO.CallStatus.Expired;
        }

        if (assignments.Any(a => a.AssignmentStatus == null)) // Active assignment
        {
            if (call.MaxCompletionTime.HasValue && (call.MaxCompletionTime.Value - systemClock).TotalDays <= 5)
            {
                return BO.CallStatus.InProgressAtRisk;
            }
            return BO.CallStatus.InProgress;
        }

        // Default case for open calls
        if (call.MaxCompletionTime.HasValue && systemClock > call.MaxCompletionTime.Value)
        {
            return BO.CallStatus.Expired;
        }

        return BO.CallStatus.Open;
    }
    internal static BO.Call ConvertCallToBO(DO.Call call)
    {
        return new BO.Call
        {
            Id = call.Id,
            CallType = MapCallType(call.CallType), 
            Description = call.Description,
            FullAddress = call.FullAddress ?? string.Empty, 
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            OpeningTime = call.OpeningTime,
            MaxCompletionTime = call.MaxCompletionTime,
            Status = GetCallStatus(call.Id), 
            CallAssigns = AssignsCall(call.Id)
        };
    }

    /// <summary>
    /// Retrieves the call in progress for a volunteer by their ID.
    /// </summary>
    /// <param name="volunteerId"></param>
    /// <returns></returns>
    internal static BO.Call GetCurrentCallInProgress(int volunteerId)
    {
        // Retrieve all assignments for the volunteer
        IEnumerable<DO.Assignment> assignments;
        lock (AdminManager.BlMutex)
            assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId &&
                                                      _dal.Call.Read(a.CallId).MaxCompletionTime > AdminManager.Now);
        // If the volunteer has no assignments, return null
        if (assignments == null)
            return null;

        //Make sure the current call is not completed
        var currentAssignment = assignments.FirstOrDefault(a => CallManager.GetCallStatus(a.CallId) != BO.CallStatus.Completed);

        // If the volunteer has no current open assignments, return null
        if (currentAssignment == null)
            return null;

        DO.Call call;
        lock (AdminManager.BlMutex)
            call = _dal.Call.Read(currentAssignment.CallId);
        return CallManager.ConvertCallToBO(call);
    }
    public static BO.CallType MapCallType(DO.CallType callType)
    {
        return callType switch
        {
            DO.CallType.CleaningShelters => BO.CallType.CleaningShelters,
            DO.CallType.FoodPackagingForNeedyFamilies => BO.CallType.FoodPackagingForNeedyFamilies,
            DO.CallType.HelpForFamiliesInNeed => BO.CallType.HelpForFamiliesInNeed,
            DO.CallType.HospitalVisitsForMoraleBoost => BO.CallType.HospitalVisitsForMoraleBoost,
            DO.CallType.Undefined => BO.CallType.Undefined,
            _ => BO.CallType.Undefined
        };
    }

    public static DO.CallType MapCallType(BO.CallType callType)
    {
        return callType switch
        {
            BO.CallType.CleaningShelters => DO.CallType.CleaningShelters,
            BO.CallType.FoodPackagingForNeedyFamilies => DO.CallType.FoodPackagingForNeedyFamilies,
            BO.CallType.HelpForFamiliesInNeed => DO.CallType.HelpForFamiliesInNeed,
            BO.CallType.HospitalVisitsForMoraleBoost => DO.CallType.HospitalVisitsForMoraleBoost,
            BO.CallType.Undefined => DO.CallType.Undefined,
            _ => DO.CallType.Undefined
        };
    }

    public static List<BO.CallAssignInList> AssignsCall(int callId)
    {
        // Retrieve the list of assignments for the call
        IEnumerable<Assignment> assignments;
        lock (AdminManager.BlMutex)
             assignments = _dal.Assignment.ReadAll(a => a.CallId == callId)?.ToList() ?? new List<Assignment>();
        //var assignments = _dal.Assignment.ReadAll(a => a.CallId == callId).ToList();
        if (!assignments.Any())
            new List<BO.CallAssignInList>();

        List<BO.CallAssignInList> callAssigns;  
        lock (AdminManager.BlMutex)
        {
             callAssigns = assignments.Select(a => new BO.CallAssignInList
            {
                VolunteerId = a.VolunteerId,
                VolunteerName = _dal.Volunteer.Read(a.VolunteerId)?.FullName,
                EntryTime = a.AdmissionTime,
                FinishTime = a.ActualEndTime,
                AssignmentStatus = a.AssignmentStatus == null ? null : AssignmentManager.MapAssignmentStatus(a.AssignmentStatus)
            }).ToList();
        }
        return callAssigns;
    }
 

    internal static void PeriodicCallsUpdates(DateTime oldClock, DateTime newClock)
    {
        IEnumerable<DO.Call> calls;
        lock (AdminManager.BlMutex)
             calls = _dal.Call.ReadAll().Where(c => c.MaxCompletionTime.HasValue && c.MaxCompletionTime.Value < newClock);

        foreach (var call in calls)
        {
            IEnumerable<Assignment> assignments;
            lock (AdminManager.BlMutex)
                 assignments = _dal.Assignment.ReadAll().Where(a => a.CallId == call.Id).ToList();

            if (!assignments.Any())
            {
                // No assignments exist, create a new one with "cancel expired" status
                var newAssignment = new Assignment
                {
                    CallId = call.Id,
                    VolunteerId = 0,
                    AdmissionTime = newClock,
                    ActualEndTime = newClock,
                    AssignmentStatus = DO.AssignmentStatus.ExpiredCancellation
                };
                lock (AdminManager.BlMutex)
                    _dal.Assignment.Create(newAssignment);
                AssignmentManager.Observers.NotifyListUpdated(); //stage 5                                                    
            }
            else
            {
                // Update existing assignment with "cancel expired" status
                var activeAssignment = assignments.FirstOrDefault(a => a.ActualEndTime == null);
                if (activeAssignment != null)
                {
                    var newAssignment = new Assignment
                    {
                        Id = activeAssignment.Id,
                        CallId = activeAssignment.CallId,
                        VolunteerId = activeAssignment.VolunteerId,
                        AdmissionTime = activeAssignment.AdmissionTime,
                        ActualEndTime = newClock,
                        AssignmentStatus = DO.AssignmentStatus.ExpiredCancellation
                    };
                    lock (AdminManager.BlMutex)
                        _dal.Assignment.Update(newAssignment);
                    AssignmentManager.Observers.NotifyItemUpdated(newAssignment.Id);  //stage 5
                    AssignmentManager.Observers.NotifyListUpdated();  //stage 5
                }
            }

        }
    }
    internal static BO.CallStatus getCallStatus(DateTime MaxCompletionTime)
    {
        lock (AdminManager.BlMutex)
        {
            if (MaxCompletionTime < AdminManager.Now)
            {
                return BO.CallStatus.Expired;
            }
            else if ((MaxCompletionTime - AdminManager.Now) <= _dal.Config.RiskRange)
            {
                return BO.CallStatus.OpenAtRisk;
            }
        }
        return BO.CallStatus.Open;
    }
    internal static bool IsCallInRiskRange(int callId)
    {
        TimeSpan riskRange;
        DO.Call call;
        lock (AdminManager.BlMutex)
        {
            riskRange = _dal.Config.RiskRange;
            call = _dal.Call.Read(callId);
        }
        var maxCompletionTime = call.MaxCompletionTime;
        if (maxCompletionTime.HasValue && (maxCompletionTime.Value - AdminManager.Now) <= riskRange)
             return true;
        
        return false;
    }
    internal static bool IsValidCall(BO.Call call)
    {
        if (call == null)
        {
            return false;
        }

        if (string.IsNullOrEmpty(call.Description) || string.IsNullOrEmpty(call.FullAddress))
        {
            return false;
        }
        if (call.MaxCompletionTime.HasValue && call.MaxCompletionTime.Value <= call.OpeningTime)
        {
            return false;
        }

        if (call.OpeningTime == default || call.MaxCompletionTime == null)
        {
            return false;
        }

        return true;
    }

  
}


