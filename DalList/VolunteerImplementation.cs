namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class VolunteerImplementation : IVolunteer
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
            throw new Exception($"Volunteer with ID={item.Id} already exists");
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
            throw new InvalidOperationException($"Object of type Volunteer with ID {id} does not exist.");
        }
    }


    /// <summary>
    /// Deletes all the volunteers
    /// </summary>
    public void DeleteAll()
    {
        foreach (Volunteer item in DataSource.Volunteers)
        {
            Delete(item.Id);
        }
    }


    /// <summary>
    /// Retrieves a volunteer by their ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer to retrieve.</param>
    /// <returns>The volunteer with the specified ID or null if the volunteer doesn't exist</returns>
    public Volunteer? Read(int id)
    {
        return DataSource.Volunteers.Find(x => x.Id == id);
    }


    /// <summary>
    /// Retrieves all volunteers from the data source.
    /// </summary>
    /// <returns>A list of all <see cref="Volunteer"/> objects.</returns>
    public List<Volunteer> ReadAll()
    {
        return new List<Volunteer>(DataSource.Volunteers);
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
            throw new InvalidOperationException($"Object of type Volunteer with ID {item.Id} does not exist.");
        }
    }
}
