namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
/// <summary>
/// The implementation of the interface IAssignment
/// </summary>
public class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// This method creates a new item and updates the database
    /// </summary>
    /// <param name="item">The new item</param>
    public void Create(Assignment item)
    {
        Assignment newAssignment = item with { Id = Config.NextAssignmentId };
        DataSource.Assignments.Add(newAssignment);
    }
    /// <summary>
    /// This function performs the deletion operation if the item exists otherwise throws an error
    /// </summary>
    /// <param name="id">The Id of the item to be deleted</param>
    /// <exception cref="InvalidOperationException">Error that the user tried to perform an illegal operation (delete an item that does not exist in the database)</exception>
    public void Delete(int id)
    {
        Assignment? assignment = Read(id);
        if (assignment is not null)   // Makes sure the item to delete exists
        {
            DataSource.Assignments.Remove(assignment);
        }
        else
        {
            throw new InvalidOperationException($"Object of type Assignment with ID {id} does not exist.");
        }
    }
    /// <summary>
    /// This function deletes the entire buffer by running the remove function on each of the items
    /// </summary>
    public void DeleteAll()
    {
        foreach (Assignment item in DataSource.Assignments)
        {
            Delete(item.Id);
        }
    }
    /// <summary>
    /// The method looks for the object in the database if it finds it it returns it otherwise it returns null
    /// </summary>
    /// <param name="id">The ID number of the item you want to read</param>
    /// <returns>the requested item or Null</returns>
    public Assignment? Read(int id)
    {
        return DataSource.Assignments.Find(x => x.Id == id);
    }
    /// <summary>
    /// Returns a copy of the original list
    /// </summary>
    public List<Assignment> ReadAll()
    {
        return new List<Assignment>(DataSource.Assignments);
    }
    /// <summary>
    /// This function is responsible for updating the database if the item to be updated exists, if it does not exist it will throw an error
    /// </summary>
    /// <param name="item">The item you want to update</param>
    /// <exception cref="InvalidOperationException">Error that the user tried to do an illegal operation (update an item that does not exist)</exception>
    public void Update(Assignment item)
    {
        Assignment? unupdatedAssignment = Read(item.Id);
        if (unupdatedAssignment is not null)
        {
            Delete(unupdatedAssignment!.Id);
            Create(item);
        }
        else                                                  // if unupdatedAssignment is null it means the item doe's not exists
        {
            throw new InvalidOperationException($"Object of type Assignment with ID {item.Id} does not exist.");
        }
    }
}
