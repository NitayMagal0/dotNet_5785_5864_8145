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

    /// <summary>
    /// Retrieves the current time from the clock.
    /// </summary>
    /// <returns>Current time</returns>
    public DateTime GetClock()
    {
        return ClockManager.Now;
    }

    /// <summary>
    /// Initializes the database 
    /// </summary>
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
            ClockManager.UpdateClock(ClockManager.Now);
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
    public TimeSpan GetMaxRange()
    {
        return _dal.Config.RiskRange;
    }

    /// <summary>
    /// Sets the maximum range for a call.
    /// </summary>
    /// <param name="maxRange">The new maximum time range for a call.</param>
    public void SetMaxRange(TimeSpan maxRange)
    {
        _dal.Config.RiskRange = maxRange;
    }
}