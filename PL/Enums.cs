using System.Collections;

namespace PL;
/// <summary>
/// Collection class for CallType enums.
/// </summary>
internal class CallsCollection : IEnumerable
{
    /// <summary>
    /// Static readonly collection of CallType enums.
    /// </summary>
    static readonly IEnumerable<BO.CallType> s_enums =
        (Enum.GetValues(typeof(BO.CallType)) as IEnumerable<BO.CallType>)!;

    /// <summary>
    /// Returns an enumerator that iterates through the CallType collection.
    /// </summary>
    /// <returns>An enumerator for the CallType collection.</returns>
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

/// <summary>
/// Collection class for CallStatus enums.
/// </summary>
internal class CallStatusCollection : IEnumerable
{
    /// <summary>
    /// Static readonly collection of CallStatus enums.
    /// </summary>
    static readonly IEnumerable<BO.CallStatus> s_enums =
        (Enum.GetValues(typeof(BO.CallStatus)) as IEnumerable<BO.CallStatus>)!;

    /// <summary>
    /// Returns an enumerator that iterates through the CallStatus collection.
    /// </summary>
    /// <returns>An enumerator for the CallStatus collection.</returns>
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

/// <summary>
/// Collection class for Role enums.
/// </summary>
internal class RolesCollection : IEnumerable
{
    /// <summary>
    /// Static readonly collection of Role enums.
    /// </summary>
    static readonly IEnumerable<BO.Role> s_enums =
        (Enum.GetValues(typeof(BO.Role)) as IEnumerable<BO.Role>)!;

    /// <summary>
    /// Returns an enumerator that iterates through the Role collection.
    /// </summary>
    /// <returns>An enumerator for the Role collection.</returns>
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
/// <summary>
/// Collection class for DistanceType enums.
/// </summary>
internal class DistanceTypeCollection : IEnumerable
{
    /// <summary>
    /// Static readonly collection of DistanceType enums.
    /// </summary>
    static readonly IEnumerable<BO.DistanceType> s_enums =
        (Enum.GetValues(typeof(BO.DistanceType)) as IEnumerable<BO.DistanceType>)!;

    /// <summary>
    /// Returns an enumerator that iterates through the DistanceType collection.
    /// </summary>
    /// <returns>An enumerator for the DistanceType collection.</returns>
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
