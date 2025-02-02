namespace BlApi;
/// <summary>
/// Interface for administrative operations in the BL layer.
/// </summary>
public interface IAdmin
{
    /// <summary>
    /// Initializes the database.
    /// </summary>
    void InitializeDB();

    /// <summary>
    /// Resets the database to its default state.
    /// </summary>
    void ResetDB();

    /// <summary>
    /// Gets the maximum range for some configuration.
    /// </summary>
    /// <returns>The maximum range as a TimeSpan.</returns>
    TimeSpan GetMaxRange();

    /// <summary>
    /// Sets the maximum range for some configuration.
    /// </summary>
    /// <param name="maxRange">The maximum range to set.</param>
    void SetMaxRange(TimeSpan maxRange);

    /// <summary>
    /// Gets the current clock value.
    /// </summary>
    /// <returns>The current clock value as a DateTime.</returns>
    DateTime GetClock();

    /// <summary>
    /// Advances the clock by a specified time unit.
    /// </summary>
    /// <param name="unit">The time unit to advance the clock by.</param>
    void ForwardClock(BO.TimeUnit unit);

    /// <summary>
    /// Starts the simulator with a specified interval.
    /// </summary>
    /// <param name="interval">The interval in milliseconds.</param>
    void StartSimulator(int interval); //stage 7

    /// <summary>
    /// Stops the simulator.
    /// </summary>
    void StopSimulator(); //stage 7

    #region Stage 5
    /// <summary>
    /// Adds an observer for configuration changes.
    /// </summary>
    /// <param name="configObserver">The action to execute when the configuration changes.</param>
    void AddConfigObserver(Action configObserver);

    /// <summary>
    /// Removes an observer for configuration changes.
    /// </summary>
    /// <param name="configObserver">The action to remove from the configuration observers.</param>
    void RemoveConfigObserver(Action configObserver);

    /// <summary>
    /// Adds an observer for clock changes.
    /// </summary>
    /// <param name="clockObserver">The action to execute when the clock changes.</param>
    void AddClockObserver(Action clockObserver);

    /// <summary>
    /// Removes an observer for clock changes.
    /// </summary>
    /// <param name="clockObserver">The action to remove from the clock observers.</param>
    void RemoveClockObserver(Action clockObserver);
    #endregion Stage 5
}

