using BO;
namespace BlApi;
/// <summary>
/// Interface for managing Volunteer entities in the BL (Business Logic) layer.
/// </summary>
public interface IVolunteer : IObservable
{
    /// <summary>
    /// Signs in a volunteer using their ID and password.
    /// </summary>
    /// <param name="id">The ID of the volunteer.</param>
    /// <param name="password">The password of the volunteer.</param>
    /// <returns>The role of the signed-in volunteer.</returns>
    Role SignIn(int id, string password);

    /// <summary>
    /// Gets a list of volunteers filtered by call type.
    /// </summary>
    /// <param name="callType">The call type to filter by.</param>
    /// <returns>An enumerable collection of volunteers.</returns>
    public IEnumerable<BO.VolunteerInList> GetVolunteersByCallType(BO.CallType callType);

    /// <summary>
    /// Gets a list of volunteers with optional filters for activity status and call type.
    /// </summary>
    /// <param name="isActive">Optional filter for active status.</param>
    /// <param name="VolunteerInList">Optional filter for call type.</param>
    /// <returns>An enumerable collection of volunteers.</returns>
    IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive, BO.CallType? VolunteerInList);

    /// <summary>
    /// Gets the details of a specific volunteer by their ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer.</param>
    /// <returns>The details of the volunteer.</returns>
    Volunteer GetVolunteerDetails(int id);

    /// <summary>
    /// Updates an existing volunteer's details.
    /// </summary>
    /// <param name="id">The ID of the volunteer.</param>
    /// <param name="vol">The volunteer entity with updated values.</param>
    void UpdateVolunteer(int id, BO.Volunteer vol);

    /// <summary>
    /// Deletes a volunteer by their ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer to delete.</param>
    void DeleteVolunteer(int id);

    /// <summary>
    /// Adds a new volunteer.
    /// </summary>
    /// <param name="vol">The volunteer entity to add.</param>
    void AddVolunteer(BO.Volunteer vol);

    /// <summary>
    /// Gets the ID of a volunteer by their name.
    /// </summary>
    /// <param name="name">The name of the volunteer.</param>
    /// <returns>The ID of the volunteer.</returns>
    int GetIdByName(string name);

    /// <summary>
    /// Gets the name of a volunteer by their ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer.</param>
    /// <returns>The name of the volunteer.</returns>
    public string GetNameById(int id);

    /// <summary>
    /// Updates the coordinates for a volunteer's address asynchronously.
    /// </summary>
    /// <param name="doVolunteer">The volunteer entity with the address to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateCoordinatesForVolunteerAddressAsync(DO.Volunteer doVolunteer);
}

