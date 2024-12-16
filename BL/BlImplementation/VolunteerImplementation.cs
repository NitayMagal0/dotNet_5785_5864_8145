namespace BlImplementation;
using Helpers;
using BlApi;
using BO;
using DO;

//need to fix the exeptions and getvolunteerdetails
internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;


    /// <summary>
    /// Adds a new volunteer to the system.
    /// </summary>
    /// <param name="volunteerToAdd">The volunteer object to be added.</param>
    /// <exception cref="Exception">Thrown when the volunteer cannot be added.</exception>
    public void AddVolunteer(BO.Volunteer volunteerToAdd)
    {
        //check if the volunteer is valid
        if (!VolunteerManager.IsValidVolunteer(volunteerToAdd))
            throw new Exception("Invalid volunteer details");

        var volunteer = VolunteerManager.ConvertVolunteerToDO(volunteerToAdd);
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
        //need to check if he has a call in progress of if he never had a call
        var volunteer = VolunteerManager.ConvertVolunteerToBO(_dal.Volunteer.Read(id));
        //Check if the volunteer has a call in progress (might not be working cause when we convert to BO im not sure whats happening to CallInProgress)
        if (volunteer.CallInProgress == null)
        {//the document says to check if he either has a call in progress or never had a call so i think its enough to check if he has a call in progress
            try
            {
                _dal.Volunteer.Delete(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Couldn't delete the volunteer", ex);
            }
        }
        else
        {
            throw new Exception("Volunteer has a call in progress");
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
            // Step 1: Retrieve the volunteer details from the DAL
            var volunteer = VolunteerManager.ConvertVolunteerToBO(_dal.Volunteer.Read(id));

            // Step 2: Retrieve the call in progress for the volunteer
            volunteer.CallInProgress = VolunteerManager.GetCallInProgress(id);

            // Step 3: Return the volunteer details
            return volunteer;
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
    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive, Enum? VolunteerInList)
    {
        try
        {
            // Step 1: Retrieve all volunteers from the DAL and convert to BO
            var volunteers = _dal.Volunteer.ReadAll().Select(VolunteerManager.ConvertVolunteerToBO).ToList();
            // Step 2: Filter the volunteers based on the isActive parameter
            var filteredVolunteers = volunteers.Where(v => isActive == null || v.IsActive == isActive).ToList();

            // Step 3: Convert the list of volunteers to a list of VolunteerInList objects
            var volunteersInList = filteredVolunteers.Select(v => new VolunteerInList
            {
                Id = v.Id,
                FullName = v.FullName,
                IsActive = v.IsActive,
                HandledCalls = v.HandledCalls,
                CanceledCalls = v.CanceledCalls,
                ExpiredCalls = v.ExpiredCalls,
                CallsInProgress = v.CallInProgress?.Id,
                CallType = v.CallInProgress?.CallType ?? BO.CallType.Undefined
            }).ToList();

            // Step 4: Sort the list of volunteers based on the VolunteerInList parameter
            if (VolunteerInList != null)
            {
                volunteersInList = volunteersInList.OrderBy(v => v.CallType == (BO.CallType)VolunteerInList).ToList();
            }
            //if the parameter is null, sort by ID
            else
            {
                volunteersInList = volunteersInList.OrderBy(v => v.Id).ToList();
            }
            // Step 5: Return the list of volunteers
            return volunteersInList;

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
    /// <param name="updatedVolunteer">The updated volunteer object.</param>
    /// <exception cref="Exception">Thrown when the volunteer cannot be updated.</exception>
    public void UpdateVolunteer(int requesterId, BO.Volunteer updatedVolunteer)
    {
        
        var requester = VolunteerManager.ConvertVolunteerToBO(_dal.Volunteer.Read(requesterId));
        if (requester == null)
        {
            throw new UnauthorizedAccessException("Requester not found");
        }
        bool isAdmin = requester.Role == BO.Role.Manager;
        bool isVolunteer = updatedVolunteer.Id == requesterId;
        //Check if the requester is an Admin/the volunteer himself
        if (!isAdmin && !isVolunteer)
        {
            throw new UnauthorizedAccessException("The requester is not authorized to cancel this assignment");
        }

        //Check if the requester is an Admin and the role of the volunteer is different
        if (!isAdmin && requester.Role != updatedVolunteer.Role)
            throw new Exception("Non Admin volunteer can't change his role");

        //Check if the updated volunteer is valid
        if (!VolunteerManager.IsValidVolunteer(updatedVolunteer))
            throw new Exception("Invalid volunteer details");
      
            updatedVolunteer.Latitude = Tools.GetCoordinates(updatedVolunteer.FullAddress).Item1;
            updatedVolunteer.Longitude = Tools.GetCoordinates(updatedVolunteer.FullAddress).Item2;
        

        try
        {
                _dal.Volunteer.Update(VolunteerManager.ConvertVolunteerToDO(updatedVolunteer));
            }
            catch (Exception ex)
            {
                throw new Exception("Couldn't update the volunteer", ex);
            }
        
    }
}

