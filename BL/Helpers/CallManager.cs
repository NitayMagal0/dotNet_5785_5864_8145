using BlApi;
using BlImplementation;
using BO;
using DO;

namespace Helpers;

internal class CallManager
{
    private static readonly DalApi.IDal _dal = DalApi.Factory.Get; //stage 4
    internal static BO.CallStatus GetCallStatus(int callId)
    {
        // Fetch the call and its related data
        var call = _dal.Call.Read(callId);
        var assignments = _dal.Assignment.ReadAll().Where(a => a.CallId == callId);
        var systemClock = ClockManager.Now;

        if (call == null)
        {
            throw new ArgumentException("Invalid call ID");
        }

        // Handle case where no assignments exist
        if (!assignments.Any())
        {
            if (call.MaxCompletionTime.HasValue && (call.MaxCompletionTime.Value - systemClock).TotalDays <= AdminImplementation.GetMaxRange())
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
        var assignments = _dal.Assignment.ReadAll(a => a.CallId == callId).ToList();
        if (!assignments.Any())
            return null;  // Or return an empty list if needed

        List<BO.CallAssignInList> callAssigns = new List<BO.CallAssignInList>();
        callAssigns = assignments.Select(a => new BO.CallAssignInList
        {
            VolunteerId = a.VolunteerId,
            VolunteerName = _dal.Volunteer.Read(a.VolunteerId)?.FullName,
            EntryTime = a.AdmissionTime,
            FinishTime = a.ActualEndTime,
            AssignmentStatus = AssignmentManager.MapAssignmentStatus(a.AssignmentStatus)
        }).ToList();

        return callAssigns;
    }


    internal static void PeriodicCallsUpdates(DateTime oldClock, DateTime newClock)
    {
        var calls = _dal.Call.ReadAll().Where(c => c.MaxCompletionTime.HasValue && c.MaxCompletionTime.Value < newClock);

        foreach (var call in calls)
        {
            var assignments = _dal.Assignment.ReadAll().Where(a => a.CallId == call.Id).ToList();

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
                _dal.Assignment.Create(newAssignment);
            }
            else
            {
                // Update existing assignment with "cancel expired" status
                var activeAssignment = assignments.FirstOrDefault(a => a.ActualEndTime == null);
                if (activeAssignment != null)
                {
                    var newAssignment = new Assignment
                    {
                        CallId = activeAssignment.CallId,
                        VolunteerId = activeAssignment.VolunteerId,
                        AdmissionTime = activeAssignment.AdmissionTime,
                        ActualEndTime = newClock,
                        AssignmentStatus = DO.AssignmentStatus.ExpiredCancellation
                    };
                    _dal.Assignment.Update(newAssignment);
                }
            }

        }
    }
}


