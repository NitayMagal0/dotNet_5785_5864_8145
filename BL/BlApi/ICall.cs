using BO;
namespace BlApi;
/// <summary>
/// Interface for managing Call entities in the BL (Business Logic) layer.
/// </summary>
public interface ICall : IObservable
{
    /// <summary>
    /// Gets the count of calls by their status.
    /// </summary>
    /// <returns>An array of integers representing the count of calls for each status.</returns>
    int[] GetCallCountsByStatus();

    /// <summary>
    /// Gets a filtered and sorted list of calls.
    /// </summary>
    /// <param name="filterField">The field to filter by.</param>
    /// <param name="filterValue">The value to filter by.</param>
    /// <param name="sortField">The field to sort by.</param>
    /// <returns>An enumerable collection of filtered and sorted calls.</returns>
    IEnumerable<CallInList> GetFilteredAndSortedCalls(CallType? filterField, CallStatus? filterValue, Enum? sortField);

    /// <summary>
    /// Gets the details of a specific call by its ID.
    /// </summary>
    /// <param name="callId">The ID of the call.</param>
    /// <returns>The details of the call.</returns>
    BO.Call GetCallDetails(int callId);

    /// <summary>
    /// Updates an existing call.
    /// </summary>
    /// <param name="call">The call entity with updated values.</param>
    void UpdateCall(BO.Call call);

    /// <summary>
    /// Deletes a call by its ID.
    /// </summary>
    /// <param name="callId">The ID of the call to delete.</param>
    void DeleteCall(int callId);

    /// <summary>
    /// Adds a new call.
    /// </summary>
    /// <param name="call">The call entity to add.</param>
    void AddCall(BO.Call call);

    /// <summary>
    /// Gets the history of closed calls for a specific volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <param name="callTypeFilter">The call type filter.</param>
    /// <param name="sortField">The field to sort by.</param>
    /// <returns>An enumerable collection of closed calls.</returns>
    IEnumerable<BO.ClosedCallInList> GetVolunteerClosedCallsHistory(int volunteerId, BO.CallType? callTypeFilter, Enum? sortField);

    /// <summary>
    /// Gets the available open calls for a specific volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <param name="callTypeFilter">The call type filter.</param>
    /// <param name="sortField">The field to sort by.</param>
    /// <returns>An enumerable collection of open calls.</returns>
    IEnumerable<BO.OpenCallInList> GetAvailableOpenCallsForVolunteer(int volunteerId, BO.CallType? callTypeFilter, Enum? sortField);

    /// <summary>
    /// Marks an assignment as completed.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <param name="assignmentId">The ID of the assignment.</param>
    void MarkAssignmentAsCompleted(int volunteerId, int assignmentId);

    /// <summary>
    /// Cancels an assignment.
    /// </summary>
    /// <param name="requesterId">The ID of the requester.</param>
    /// <param name="callId">The ID of the call.</param>
    void CancelAssignment(int requesterId, int callId);

    /// <summary>
    /// Assigns a call to a volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <param name="callId">The ID of the call.</param>
    void AssignCallToVolunteer(int volunteerId, int callId);

    /// <summary>
    /// Gets the calls in progress for a specific volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <returns>An enumerable collection of calls in progress.</returns>
    public IEnumerable<BO.CallInProgress> GetCallsForVolunteer(int volunteerId);

    /// <summary>
    /// Gets a call in list by its ID.
    /// </summary>
    /// <param name="callId">The ID of the call.</param>
    /// <returns>The call in list.</returns>
    public CallInList GetCallInListById(int callId);

    /// <summary>
    /// Adds a minimal call with the specified details.
    /// </summary>
    /// <param name="CallType">The type of the call.</param>
    /// <param name="Description">The description of the call.</param>
    /// <param name="FullAddress">The full address of the call.</param>
    /// <param name="MaxCompletionTime">The maximum completion time for the call.</param>
    public void MinAddCall(BO.CallType CallType, string Description, string FullAddress, DateTime? MaxCompletionTime);

    /// <summary>
    /// Gets the nearby open calls for a volunteer within a specified range.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <param name="range">The range to search within.</param>
    /// <param name="distanceType">The type of distance calculation.</param>
    /// <param name="callTypeFilter">The call type filter.</param>
    /// <param name="sortField">The field to sort by.</param>
    /// <returns>An enumerable collection of nearby open calls.</returns>
    public IEnumerable<OpenCallInList> GetNearbyOpenCallsForVolunteer(int volunteerId, double range, DistanceType distanceType, CallType? callTypeFilter, Enum? sortField);
}

