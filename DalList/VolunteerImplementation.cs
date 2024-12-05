namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

internal class VolunteerImplementation : IVolunteer
{
    /// <summary>
    /// Adds a new volunteer to the data source.
    /// </summary>
    /// <param name="item">The <see cref="Volunteer"/> object to add.</param>
    /// <exception cref="Exception">
    /// Thrown if a volunteer with the same ID already exists in the data source.
    /// </exception>
    public void Create(Volunteer item)
    {
        if (Read(item.Id) is not null)
            throw new DalEntityAlreadyExistsException($"Volunteer with ID={item.Id} already exists");
        else
            DataSource.Volunteers.Add(item);
    }


    /// <summary>
    /// Deletes a volunteer by their ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer to delete</param>
    /// <exception cref="Exception">Thrown if no volunteer with the specified ID is found.</exception>
    public void Delete(int id)
    {
        Volunteer? volunteer = Read(id);
        if (volunteer is not null)   // Makes sure the item to delete exists
        {
            DataSource.Volunteers.Remove(volunteer);
        }
        else
        {
            throw new DalDoesNotExistException($"Object of type Volunteer with ID {id} does not exist.");
        }
    }


    /// <summary>
    /// Deletes all the volunteers
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
        /*var volunteersCopy = DataSource.Volunteers.ToList();
        foreach (Volunteer item in DataSource.Volunteers)
        {
            Delete(item.Id);
        }*/
    }


    /// <summary>
    /// Retrieves a volunteer by their ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer to retrieve.</param>
    /// <returns>The volunteer with the specified ID or null if the volunteer doesn't exist</returns>
    public Volunteer? Read(int id)
    {
        return DataSource.Volunteers.FirstOrDefault(x => x.Id == id);
    }


    /// <summary>
    /// Retrieves all volunteers from the data source.
    /// </summary>
    /// <returns>A list of all <see cref="Volunteer"/> objects.</returns>
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        return filter == null ? DataSource.Volunteers : DataSource.Volunteers.Where(filter);
    }


    /// <summary>
    /// Updates the details of an existing volunteer by first deleting the current record and then creating a new record with the updated details.
    /// </summary>
    /// <param name="item">object containing the updated details of the volunteer</param>
    public void Update(Volunteer item)
    {
        Volunteer? unupdatedVolunteer = Read(item.Id);
        if (unupdatedVolunteer is not null)
        {
            Delete(unupdatedVolunteer!.Id);
            Create(item);
        }
        else                                                  // if unupdatedVolunteer is null it means the item doe's not exists
        {
            throw new DalDoesNotExistException($"Object of type Volunteer with ID {item.Id} does not exist.");
        }
    }

    /// <summary>
    /// Retrieves a volunteer by a specified filter.
    /// </summary>
    /// <param name="filter">The filter to apply to find the volunteer.</param>
    /// <returns>The volunteer that matches the filter or null if no volunteer matches.</returns>
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return DataSource.Volunteers.FirstOrDefault(filter);
    }
}
