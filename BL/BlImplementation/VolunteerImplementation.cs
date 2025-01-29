namespace BlImplementation;
using Helpers;
using BlApi;
using BO;
using DO;
using DalApi;
using System;
using DalApi;

internal class VolunteerImplementation : BlApi.IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;


    /// <summary>
    /// Adds a new volunteer to the system.
    /// </summary>
    /// <param name="volunteerToAdd">The volunteer object to be added.</param>
    /// <exception cref="Exception">Thrown when the volunteer cannot be added.</exception>
    public void AddVolunteer(BO.Volunteer volunteerToAdd)
    {
        // 🔹 Check if the simulator is running before proceeding
        AdminManager.ThrowOnSimulatorIsRunning();

        // 🔹 Validate the volunteer details to ensure they meet all criteria
        if (!VolunteerManager.IsValidVolunteer(volunteerToAdd))
        {
            throw new Exception("Invalid volunteer details.");
        }

        // 🔹 Encode the password before saving the volunteer to the database
        // This ensures security by storing an encoded version instead of plain text.
        volunteerToAdd.Password = VolunteerManager.EncodePassword(volunteerToAdd.Password);

        // 🔹 Convert the Business Object (BO) volunteer to a Data Object (DO)
        // Note: We initially **do not compute** the coordinates in this step.
        var doVolunteer = VolunteerManager.ConvertVolunteerToDO(volunteerToAdd);

        // 🔹 Set initial Latitude & Longitude to `null`
        // This allows us to store the volunteer immediately without waiting for coordinate calculations.
        doVolunteer = doVolunteer with { Latitude = null, Longitude = null };

        try
        {
            // 🔹 Lock to ensure thread safety when interacting with the DAL
            // This prevents race conditions when multiple threads try to modify data.
            lock (AdminManager.BlMutex)
                _dal.Volunteer.Create(doVolunteer);

            // 🔹 Notify observers that a new volunteer was added
            // This ensures the UI and other components are updated in response to the change.
            VolunteerManager.Observers.NotifyListUpdated();

            // 🔹 Compute the coordinates asynchronously (without blocking the main process)
            // We call `UpdateCoordinatesForVolunteerAddressAsync`, but **do not wait for it** (`_ =` ignores the return value).
            // This allows the volunteer to be added immediately, while the coordinates update in the background.
            _ = UpdateCoordinatesForVolunteerAddressAsync(doVolunteer);
        }
        catch (Exception ex)
        {
            // 🔹 Catch any errors that occur while adding the volunteer
            // If the DAL throws an exception, we wrap it in a higher-level exception to provide better error handling.
            throw new BlInvalidOperationException($"Failed to add volunteer: {ex.Message}");
        }
    }



    public string GetNameById(int id)
    {
        lock (AdminManager.BlMutex)
            return VolunteerManager.ConvertVolunteerToBO(_dal.Volunteer.Read(id)).FullName;
    }
    /// <summary>
    /// Deletes a volunteer from the system.
    /// </summary>
    /// <param name="id">The ID of the volunteer to be deleted.</param>
    /// <exception cref="Exception">Thrown when the volunteer cannot be deleted.</exception>
    public void DeleteVolunteer(int id)
    {
        //need to check if he has a call in progress of if he never had a call
        BO.Volunteer volunteer = null;
        lock (AdminManager.BlMutex)
             volunteer = VolunteerManager.ConvertVolunteerToBO(_dal.Volunteer.Read(id));
        //Check if the volunteer has a call in progress (might not be working cause when we convert to BO im not sure whats happening to CallInProgress)
        if (volunteer.CallInProgress == null)
        {//the document says to check if he either has a call in progress or never had a call so i think its enough to check if he has a call in progress
            try
            {
                lock (AdminManager.BlMutex)
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

            // Step 1: Retrieve the volunteer details from the DAL
            BO.Volunteer volunteer = null;
            lock (AdminManager.BlMutex)
                 volunteer = VolunteerManager.ConvertVolunteerToBO(_dal.Volunteer.Read(id));
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
    /// Retrieves the ID of a volunteer by their name.
    /// </summary>
    /// <param name="name">The name of the volunteer.</param>
    /// <returns>The ID of the volunteer.</returns>
    public int GetIdByName(string name)
    {
        return VolunteerManager.ConvertVolunteerToBO(DalApi.Factory.Get.Volunteer.ReadAll().FirstOrDefault(v => v.FullName == name)).Id;
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
            IEnumerable<BO.Volunteer> volunteers = null;
            lock (AdminManager.BlMutex)
            {
                volunteers = _dal.Volunteer.ReadAll()
               .Select(VolunteerManager.ConvertVolunteerToBO)
               .Where(v => !isActive.HasValue || v.IsActive == isActive)
               .ToList();
            }
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
                CallType = v.CallInProgress?.CallType ?? BO.CallType.Undefined,
                Latitude = v.Latitude,  
                Longitude = v.Longitude
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
            IEnumerable<BO.Volunteer> volunteers = null;
            lock (AdminManager.BlMutex)
            {
                volunteers = _dal.Volunteer.ReadAll()
               .Select(VolunteerManager.ConvertVolunteerToBO)
               .ToList();
            }
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
                    CallType = v.CallInProgress?.CallType ?? BO.CallType.Undefined,
                      Latitude = v.Latitude,
                 Longitude = v.Longitude
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
    public BO.Role SignIn(int id, string password)
    {
        IEnumerable<DO.Volunteer> volunteers = null;
        lock (AdminManager.BlMutex)
             volunteers = _dal.Volunteer.ReadAll().ToList();
        DO.Volunteer volunteer = null;
        lock (AdminManager.BlMutex)
             volunteer = volunteers.FirstOrDefault(v => v.Id == id);
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
        // Ensure the simulator is not running before making changes
        AdminManager.ThrowOnSimulatorIsRunning();

        // Retrieve the requester's details (admin or volunteer trying to update their own info)
        BO.Volunteer requester;
        lock (AdminManager.BlMutex)
            requester = VolunteerManager.ConvertVolunteerToBO(_dal.Volunteer.Read(requesterId));

        if (requester == null)
            throw new UnauthorizedAccessException("Requester not found");

        bool isAdmin = requester.Role == BO.Role.Manager;
        bool isVolunteer = updatedVolunteer.Id == requesterId;

        // Ensure that only an admin or the volunteer themselves can perform the update
        if (!isAdmin && !isVolunteer)
            throw new UnauthorizedAccessException("The requester is not authorized to update this volunteer");

        // Prevent non-admin volunteers from changing their role
        if (!isAdmin && requester.Role != updatedVolunteer.Role)
            throw new Exception("Non-admin volunteers cannot change their role");

        // Validate volunteer details before proceeding with the update
        if (!VolunteerManager.IsValidVolunteer(updatedVolunteer))
            throw new Exception("Invalid volunteer details");

        // Encode password before storing it in the database
        updatedVolunteer.Password = VolunteerManager.EncodePassword(updatedVolunteer.Password);

        // Convert BO.Volunteer to DO.Volunteer (without computing coordinates)
        DO.Volunteer doVolunteer = VolunteerManager.ConvertVolunteerToDO(updatedVolunteer);

        // Ensure that latitude and longitude are set to null initially
        // This prevents outdated or incorrect coordinates from being used before recalculating
        doVolunteer = doVolunteer with { Latitude = null, Longitude = null };

        try
        {
            // First update: Save the volunteer in the database **without coordinates**
            lock (AdminManager.BlMutex)
                _dal.Volunteer.Update(doVolunteer);

            // Notify observers that the volunteer list has been updated (UI or other components)
            VolunteerManager.Observers.NotifyItemUpdated(updatedVolunteer.Id);
            VolunteerManager.Observers.NotifyListUpdated();
        }
        catch (Exception ex)
        {
            throw new Exception("Couldn't update the volunteer", ex);
        }

        // Second update (async): Compute and update coordinates in the background
        // `_ =` means we **fire-and-forget** this task without waiting for it
        _ = UpdateCoordinatesForVolunteerAddressAsync(doVolunteer);
    }

    /// <summary>
    /// Asynchronously retrieves and updates the volunteer's coordinates after the initial update.
    /// This ensures that the database is updated as soon as the coordinates are available.
    /// </summary>
    /// <param name="doVolunteer">The volunteer entity that needs coordinates.</param>
    public async Task UpdateCoordinatesForVolunteerAddressAsync(DO.Volunteer doVolunteer)
    {
        if (!string.IsNullOrWhiteSpace(doVolunteer.FullAddress))
        {
            try
            {
                // Asynchronously fetch the coordinates based on the given address
                (double calcLatitude, double calcLongitude) = await Tools.GetCoordinatesAsync(doVolunteer.FullAddress);

                // Update the volunteer with the newly calculated coordinates
                // Using `with` ensures immutability for the record type
                doVolunteer = doVolunteer with { Latitude = calcLatitude, Longitude = calcLongitude };

                // Save the updated volunteer (with coordinates) to the database
                lock (AdminManager.BlMutex)
                    _dal.Volunteer.Update(doVolunteer);

                // Notify observers that the volunteer data has changed again
                VolunteerManager.Observers.NotifyItemUpdated(doVolunteer.Id);
                VolunteerManager.Observers.NotifyListUpdated();
            }
            catch (Exception ex)
            {
                // Log the error but do not interrupt the main update process
                Console.WriteLine($"Failed to update coordinates for volunteer {doVolunteer.Id}: {ex.Message}");
            }
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

