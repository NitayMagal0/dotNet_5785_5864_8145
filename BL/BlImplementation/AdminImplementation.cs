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
        // Check if the simulator is running
        AdminManager.ThrowOnSimulatorIsRunning();

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
    public void InitializeDB() //stage 4
    {
        // Check if the simulator is running
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.InitializeDB(); //stage 7
    }


    /// <summary>
    /// Resets the database
    /// </summary>
    public void ResetDB() //stage 4
    {
        // Check if the simulator is running
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.ResetDB(); //stage 7
    }
    /// <summary>
    /// Starts the simulator with the specified interval.
    /// </summary>
    /// <param name="interval">The interval in milliseconds at which the simulator should run.</param>
    public void StartSimulator(int interval)  //stage 7
    {
        // Check if the simulator is running
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        // Start the simulator with the given interval
        AdminManager.Start(interval); //stage 7
    }
    /// <summary>
    /// Stops the simulator.
    /// </summary>
    public void StopSimulator()
    {
        // Stop the simulator
        AdminManager.Stop(); //stage 7
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

    public void SetMaxRange(TimeSpan maxRange)
    {
        // Check if the simulator is running
        AdminManager.ThrowOnSimulatorIsRunning();

        AdminManager.RiskRange = maxRange;
    }


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