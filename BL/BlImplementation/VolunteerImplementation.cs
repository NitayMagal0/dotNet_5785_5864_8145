namespace BlImplementation;
using Helpers;
using BlApi;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;


    /// <summary>
    /// Adds a new volunteer to the system.
    /// </summary>
    /// <param name="vol">The volunteer object to be added.</param>
    /// <exception cref="Exception">Thrown when the volunteer cannot be added.</exception>
    public void AddVolunteer(BO.Volunteer vol)
    {
        var volunteer = VolunteerManager.ConvertVolunteerToDO(vol);
        try
        {
            _dal.Volunteer.Create(volunteer);
        }
        catch (Exception ex)
        {
            throw new Exception("Couldn't add the volunteer", ex);
        }
    }

    /// <summary>
    /// Deletes a volunteer from the system.
    /// </summary>
    /// <param name="id">The ID of the volunteer to be deleted.</param>
    /// <exception cref="Exception">Thrown when the volunteer cannot be deleted.</exception>
    public void DeleteVolunteer(int id)
    {
        try
        {
            var volunteers = _dal.Volunteer.ReadAll().ToList();
            volunteers.RemoveAll(v => v.Id == id);
        }
        catch (Exception ex)
        {
            throw new Exception("Couldn't delete the volunteer", ex);
        }
    }

    /// <summary>
    /// Retrieves the details of a volunteer by their ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer to retrieve details for.</param>
    /// <returns>The volunteer details as a BO.Volunteer object.</returns>
    /// <exception cref="Exception">Thrown when the volunteer with the specified ID does not exist.</exception>
    public BO.Volunteer GetVolunteerDetails(int id)
    {
        try
        {
            var volunteer = _dal.Volunteer.Read(id);
            var BOvolunteer = VolunteerManager.ConvertVolunteerToBO(volunteer);
            return BOvolunteer;
        }
        catch (Exception ex)
        {
            throw new Exception($"Volunteer with id {id} doesn't exist", ex);
        }
    }

    /// <summary>
    /// Retrieves a list of volunteers based on their active status .
    /// </summary>
    /// <param name="isActive">Optional parameter to filter volunteers by their active status.</param>
    /// <param name="VolunteerInList">Optional enumeration parameter to filter volunteers.</param>
    /// <returns>A list of volunteers matching the specified criteria.</returns>
    /// <exception cref="Exception">Thrown when there is an error retrieving the volunteers list.</exception>
    public IEnumerable<BO.Volunteer> GetVolunteersList(bool? isActive, Enum? VolunteerInList)
    {
        try
        {
            // Step 1: Retrieve all volunteers from the DAL
            var volunteers = _dal.Volunteer.ReadAll().ToList();

            // Step 2: Filter the volunteers based on the isActive parameter
            var filteredVolunteers = volunteers.Where(v => isActive == null || v.IsActive == isActive).ToList();

            // Step 3: Convert each filtered volunteer to a BO volunteer
            var volunteersList = new List<BO.Volunteer>();
            foreach (var volunteer in filteredVolunteers)
            {
                // Use VolunteerManager to convert DAL volunteer to BO volunteer
                volunteersList.Add(VolunteerManager.ConvertVolunteerToBO(volunteer));
            }

            // Step 4: Return the list of BO volunteers
            return volunteersList;
        }
        catch (Exception ex)
        {
            throw new Exception("Couldn't get the volunteers list", ex);
        }
    }

    /// <summary>
    /// Retrieve the role of a volunteer based on their name and password.
    /// </summary>
    /// <param name="name">Name of the volunteer</param>
    /// <param name="password">Password of the volunteer</param>
    /// <returns>The Role of the volunteer</returns>
    /// <exception cref="Exception">If there is no volunteer with this name of if the password is incorrect</exception>
    public BO.Role SignIn(string name, string password)
    {
        var volunteers = _dal.Volunteer.ReadAll().ToList();
        var volunteer = volunteers.FirstOrDefault(v => v.FullName == name);
        if (volunteer == null)
            throw new Exception("Volunteer doesn't exist");
        if (volunteer.Password != password)
            throw new Exception("Password is incorrect");
        return VolunteerManager.MapRole(volunteer.Role);
    }

    /// <summary>
    /// Updates the details of an existing volunteer.
    /// </summary>
    /// <param name="id">The ID of the volunteer to be updated.</param>
    /// <param name="vol">The updated volunteer object.</param>
    /// <exception cref="Exception">Thrown when the volunteer cannot be updated.</exception>
    public void UpdateVolunteer(int id, BO.Volunteer vol)
    {
        try
        {
            _dal.Volunteer.Update(VolunteerManager.ConvertVolunteerToDO(vol));
        }
        catch (Exception ex)
        {
            throw new Exception("Couldn't update the volunteer", ex);
        }
    }
}

