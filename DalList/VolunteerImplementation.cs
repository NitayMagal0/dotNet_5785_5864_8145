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

        DataSource.Volunteers.Add(item);
    }


    /// <summary>
    /// Deletes a volunteer by their ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer to delete</param>
    /// <exception cref="Exception">Thrown if no volunteer with the specified ID is found.</exception>
    public void Delete(int id)
    {
        Volunteer temp = Read(id);
        if (temp is not null)
            throw new Exception($"Volunteer with ID={id} doesn't exist");
        DataSource.Volunteers.Remove(temp);
    }

    /// <summary>
    /// Deletes all the volunteers
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }

    /// <summary>
    /// Retrieves a volunteer by their ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer to retrieve.</param>
    /// <returns>The volunteer with the specified ID.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if no volunteer with the specified ID is found.</exception>
    public Volunteer? Read(int id)
    {
        var volunteer = DataSource.Volunteers.FirstOrDefault(v => v.Id == id);

        if (volunteer == null)
        {
            throw new KeyNotFoundException($"Volunteer with ID {id} not found.");
        }

        return volunteer;
    }

    /// <summary>
    /// Retrieves all volunteers from the data source.
    /// </summary>
    /// <returns>A list of all <see cref="Volunteer"/> objects.</returns>
    public List<Volunteer> ReadAll()
    {
        return DataSource.Volunteers;
    }


    /// <summary>
    /// Updates the details of an existing volunteer by first deleting the current record and then creating a new record with the updated details.
    /// </summary>
    /// <param name="item">object containing the updated details of the volunteer</param>
    public void Update(Volunteer item)
    {
        Delete(item.Id);
        Create(item);
    }
}
