namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        Assignment newAssignment = item with { Id = Config.NextAssignmentId };
        DataSource.Assignments.Add(newAssignment);
    }

    public void Delete(int id)
    {
        Assignment? Assignment = Read(id);
        if (Assignment != null)
        {
            DataSource.Assignments.Remove(Assignment);
        }
        else
        {
            throw new InvalidOperationException($"Object of type Assignment with ID {id} does not exist.");
        }
    }

    public void DeleteAll()
    {
        foreach (Assignment item in DataSource.Assignments)
        {
            Delete(item.Id);
        }
    }

    public Assignment? Read(int id)
    {
        return DataSource.Assignments.Find(x => x.Id == id);
    }

    public List<Assignment> ReadAll()
    {
        return new List<Assignment>(DataSource.Assignments);
    }

    public void Update(Assignment item)
    {
        Assignment? unupdatedAssignment = Read(item.Id);
        if (unupdatedAssignment == null)
            throw new InvalidOperationException($"Object of type Assignment with ID {item.Id} does not exist.");
        else
        {
            Delete(unupdatedAssignment.Id);
            Create(item);
        }
    }
}
