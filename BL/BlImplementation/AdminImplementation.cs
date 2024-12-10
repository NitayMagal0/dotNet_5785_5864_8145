namespace BlImplementation;
using BlApi;
using Helpers;

internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void ForwardClock(BO.TimeUnit unit)
    {
        switch (unit)
        {
            case BO.TimeUnit.second:
                ClockManager.UpdateClock(ClockManager.Now.AddSeconds(1));
                break;

            case BO.TimeUnit.minute:
                ClockManager.UpdateClock(ClockManager.Now.AddMinutes(1));
                break;

            case BO.TimeUnit.hour:
                ClockManager.UpdateClock(ClockManager.Now.AddHours(1));
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(unit), "Unsupported time unit");
        }
    }

    public DateTime GetClock()
    {
        return ClockManager.Now;
    }

    public void InitializeDB()
    {
        try
        {
            DalTest.Initialization.Do();
            ClockManager.UpdateClock(ClockManager.Now);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while Initializing the database: {ex.Message}");
        }

    }

    public void ResetDB()
    {
        try
        {
            // Reset the database
            _dal.ResetDB();
            // Update the clock
            ClockManager.UpdateClock(ClockManager.Now);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while resetting the database: {ex.Message}");
        }
    }

    public TimeSpan GetMaxRange()
    {
        return _dal.Config.RiskRange;
    }

    public void SetMaxRange(TimeSpan maxRange)
    {
        _dal.Config.RiskRange = maxRange;
    }
}