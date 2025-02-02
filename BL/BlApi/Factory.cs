namespace BlApi;
/// <summary>
/// Factory class for creating instances of the IBl interface.
/// </summary>
public static class Factory
{
    /// <summary>
    /// Creates and returns an instance of the IBl implementation.
    /// </summary>
    /// <returns>An instance of the IBl implementation.</returns>
    public static IBl Get() => new BlImplementation.Bl();
}
