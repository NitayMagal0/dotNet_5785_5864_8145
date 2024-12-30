namespace BlImplementation;
using BlApi;
using Helpers;

internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// Forwards the clock by the specified time unit.
    /// </summary>
    /// <param name="unit">The time unit to forward the clock by.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an unsupported time unit is provided.</exception>
    public void ForwardClock(BO.TimeUnit unit)
    {
        switch (unit)
        {
            case BO.TimeUnit.second:
                AdminManager.UpdateClock(AdminManager.Now.AddSeconds(1));
                break;

            case BO.TimeUnit.minute:
                AdminManager.UpdateClock(AdminManager.Now.AddMinutes(1));
                break;

            case BO.TimeUnit.hour:
                AdminManager.UpdateClock(AdminManager.Now.AddHours(1));
                break;
            case BO.TimeUnit.day:
                AdminManager.UpdateClock(AdminManager.Now.AddDays(1));
                break;
            case BO.TimeUnit.month:
                AdminManager.UpdateClock(AdminManager.Now.AddMonths(1));
                break;
            case BO.TimeUnit.year:
                AdminManager.UpdateClock(AdminManager.Now.AddYears(1));
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(unit), "Unsupported time unit");
        }
    }

    /// <summary>
    /// Retrieves the current time from the clock.
    /// </summary>
    /// <returns>Current time</returns>
    public DateTime GetClock()
    {
        return AdminManager.Now;
    }

    /// <summary>
    /// Initializes the database 
    /// </summary>
    public void InitializeDB()
    {
        try
        {
            DalTest.Initialization.Do();
            AdminManager.UpdateClock(AdminManager.Now);
            AdminManager.RiskRange = AdminManager.RiskRange;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while Initializing the database: {ex.Message}");
        }

    }

    /// <summary>
    /// Resets the database
    /// </summary>
    public void ResetDB()
    {
        try
        {
            // Reset the database
            _dal.ResetDB();
            // Update the clock
            AdminManager.UpdateClock(AdminManager.Now);
            AdminManager.RiskRange = AdminManager.RiskRange;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while resetting the database: {ex.Message}");
        }
    }

    /// <summary>
    /// Retrieves the maximum range for a call.
    /// </summary>
    /// <returns>Maximum time range</returns>

    public TimeSpan GetMaxRange() => AdminManager.RiskRange;
    /// <summary>
    /// Sets the maximum range for a call.
    /// </summary>
    /// <param name="maxRange">The new maximum time range for a call.</param>

    public void SetMaxRange(TimeSpan maxRange) => AdminManager.RiskRange = maxRange;


    #region Stage 5
    public void AddClockObserver(Action clockObserver) =>
        AdminManager.ClockUpdatedObservers += clockObserver;
    public void RemoveClockObserver(Action clockObserver) =>
        AdminManager.ClockUpdatedObservers -= clockObserver;
    public void AddConfigObserver(Action configObserver) =>
        AdminManager.ConfigUpdatedObservers += configObserver;
    public void RemoveConfigObserver(Action configObserver) =>
        AdminManager.ConfigUpdatedObservers -= configObserver;
    #endregion Stage 5
}