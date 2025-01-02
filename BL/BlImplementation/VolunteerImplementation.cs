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
        try
        {
            VolunteerManager.IsValidVolunteer(volunteerToAdd);
        }
        catch (Exception ex)
        {

            throw new Exception(ex.Message);
        }
        // Encode the password before adding the volunteer
        volunteerToAdd.Password = VolunteerManager.EncodePassword(volunteerToAdd.Password);
        // Convert the BO to DO and add the volunteer
        var volunteer = VolunteerManager.ConvertVolunteerToDO(volunteerToAdd);

        try
        {
            var coordinates = Tools.GetCoordinates(volunteer.FullAddress);
            var updatedVolunteer = volunteer with
            {
                Latitude = coordinates.Item1,
                Longitude = coordinates.Item2
            };

            _dal.Volunteer.Create(updatedVolunteer);
            VolunteerManager.Observers.NotifyListUpdated(); //stage 5
        }
        catch (Exception ex)
        {
            throw new BlInvalidOperationException(ex.Message);
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
                VolunteerManager.Observers.NotifyListUpdated();  //stage 5  	
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
            //var dovolunteer = _dal.Volunteer.Read(id);
            //Console.WriteLine("Name "+dovolunteer.FullName);
            // Step 1: Retrieve the volunteer details from the DAL
            var volunteer = VolunteerManager.ConvertVolunteerToBO(_dal.Volunteer.Read(id));
            // Step 2: Retrieve the call in progress for the volunteer
            try
            {
                volunteer.CallInProgress = VolunteerManager.GetCallInProgress(id);

            }
            catch (Exception)
            {


            }
            // Decode the password before returning the volunteer
            volunteer.Password = VolunteerManager.DecodePassword(volunteer.Password);
            // Step 3: Return the volunteer details
            return volunteer;
        }
        catch (Exception ex)
        {
            // Print the error message to the output pane
            Console.WriteLine($"ERROR: {ex.Message}");
            throw new Exception($"ERROR: ", ex);
        }
    }


    /// <summary>
    /// Retrieves a list of volunteers based on their active status and other criteria.
    /// </summary>
    /// <param name="isActive">Optional parameter to filter volunteers by their active status.</param>
    /// <param name="filterCallType">Optional parameter to filter volunteers by call type.</param>
    /// <returns>A list of volunteers matching the specified criteria.</returns>
    /// <exception cref="Exception">Thrown when there is an error retrieving the volunteers list.</exception>
    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive = null, BO.CallType? filterCallType = null)
    {
        try
        {
            // Retrieve and convert all volunteers from the DAL
            var volunteers = _dal.Volunteer.ReadAll()
                .Select(VolunteerManager.ConvertVolunteerToBO)
                .Where(v => !isActive.HasValue || v.IsActive == isActive)
                .ToList();

            // Map to VolunteerInList objects
            var volunteersInList = volunteers.Select(v => new BO.VolunteerInList
            {
                Id = v.Id,
                FullName = v.FullName,
                IsActive = v.IsActive,
                HandledCalls = v.HandledCalls,
                CanceledCalls = v.CanceledCalls,
                ExpiredCalls = v.ExpiredCalls,
                CurrentCallId = VolunteerManager.GetCallInProgress(v.Id)?.CallId,
                CallType = v.CallInProgress?.CallType ?? BO.CallType.Undefined
            });

            // Apply sorting
            if (filterCallType.HasValue)
            {
                volunteersInList = volunteersInList
                    .OrderByDescending(v => v.CallType == filterCallType)
                    .ThenBy(v => v.Id);
            }
            else
            {
                volunteersInList = volunteersInList.OrderBy(v => v.Id);
            }

            return volunteersInList;
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred while retrieving the volunteers list: {ex.Message}";
            Console.WriteLine(errorMessage);
            throw new Exception(errorMessage, ex);
        }
    }



    //-----------------------------------------------------------------------------------

    /// <summary>
    /// Retrieves a list of volunteers filtered by a specific call type.
    /// </summary>
    /// <param name="callType">The call type to filter volunteers by.</param>
    /// <returns>A list of volunteers associated with the specified call type.</returns>
    /// <exception cref="Exception">Thrown when there is an error retrieving the volunteers list.</exception>
    public IEnumerable<BO.VolunteerInList> GetVolunteersByCallType(BO.CallType callType)
    {
        try
        {
            // Retrieve and convert all volunteers from the DAL
            var volunteers = _dal.Volunteer.ReadAll()
                .Select(VolunteerManager.ConvertVolunteerToBO)
                .ToList();

            // Map to VolunteerInList objects and filter by the specified call type
            var volunteersInList = volunteers.Select(v => new BO.VolunteerInList
                {
                    Id = v.Id,
                    FullName = v.FullName,
                    IsActive = v.IsActive,
                    HandledCalls = v.HandledCalls,
                    CanceledCalls = v.CanceledCalls,
                    ExpiredCalls = v.ExpiredCalls,
                    CurrentCallId = VolunteerManager.GetCallInProgress(v.Id)?.CallId,
                    CallType = v.CallInProgress?.CallType ?? BO.CallType.Undefined
                })
                .Where(v => v.CallType == callType)
                .OrderBy(v => v.Id);

            return volunteersInList;
        }
        catch (Exception ex)
        {
            var errorMessage = $"An error occurred while retrieving the volunteers filtered by call type: {ex.Message}";
            Console.WriteLine(errorMessage);
            throw new Exception(errorMessage, ex);
        }
    }

    //-----------------------------------------------------------------------------------


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
        if (VolunteerManager.DecodePassword(volunteer.Password) != password)
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
        // Encode the password before updating the volunteer
        updatedVolunteer.Password = VolunteerManager.EncodePassword(updatedVolunteer.Password);


        try
        {
            _dal.Volunteer.Update(VolunteerManager.ConvertVolunteerToDO(updatedVolunteer));
            VolunteerManager.Observers.NotifyItemUpdated(updatedVolunteer.Id);  //stage 5
            VolunteerManager.Observers.NotifyListUpdated();  //stage 5
        }
        catch (Exception ex)
        {
            throw new Exception("Couldn't update the volunteer", ex);
        }

    }


    #region Stage 5
    public void AddObserver(Action listObserver) =>
        VolunteerManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
        VolunteerManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
        VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
        VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5
}

