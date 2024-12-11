using BO;
using DalApi;

namespace Helpers;

internal class CallManager
{
    private static readonly DalApi.IDal _dal = DalApi.Factory.Get; //stage 4
    internal static void PeriodicCallsUpdates(DateTime oldClock, DateTime newClock)
    {
        var list = _dal.Call.ReadAll().ToList();
        foreach (var doCall in list)
        {
            // Update the status of the call based on certain conditions
            if (doCall.Status == CallStatus.Open && (newClock - doCall.CreationDate).TotalDays > _dal.Config.MaxOpenDays)
            {
                _dal.Call.Update(doCall with { Status = CallStatus.OpenAtRisk });
            }
            else if (doCall.Status == CallStatus.InProgress && (newClock - doCall.CreationDate).TotalDays > s_dal.Config.MaxInProgressDays)
            {
                _dal.Call.Update(doCall with { Status = CallStatus.InProgressAtRisk });
            }
            else if ((newClock - doCall.CreationDate).TotalDays > _dal.Config.MaxTotalDays)
            {
                _dal.Call.Update(doCall with { Status = CallStatus.Expired });
            }
        }
    }
}

