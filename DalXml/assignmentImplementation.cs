namespace Dal;

using System;
using System.Collections.Generic;
using DalApi;
using DO;

internal class assignmentImplementation : IAssignment
{

    /// <summary>
    /// Creates a new assignment and saves it in the XML file.
    /// </summary>
    /// <param name="item">The assignment to create.</param>
    public void Create(Assignment item)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        Assignment newAssignment = item with { Id = XMLTools.GetAndIncreaseConfigIntVal("config.xml", "NextAssignmentId") };
        assignments.Add(newAssignment);
        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignments_xml);
    }

    /// <summary>
    /// Deletes an assignment by ID from the XML file.
    /// </summary>
    /// <param name="id">The ID of the assignment to delete.</param>
    public void Delete(int id)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);

        Assignment? assignment = assignments.FirstOrDefault(a => a.Id == id);
        if (assignment == null)
            throw new DalDoesNotExistException($"Assignment with ID={id} does not exist.");

        assignments.Remove(assignment);
        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignments_xml);
    }

    /// <summary>
    /// Deletes all assignments from the XML file.
    /// </summary>
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_assignments_xml);
    }

    /// <summary>
    /// Reads an assignment by ID from the XML file.
    /// </summary>
    /// <param name="id">The ID of the assignment to read.</param>
    /// <returns>The assignment with the specified ID.</returns>
    public Assignment Read(int id)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        return assignments.FirstOrDefault(a => a.Id == id)
               ?? throw new DalDoesNotExistException($"Assignment with ID={id} does not exist.");
    }

    /// <summary>
    /// Reads an assignment that matches the specified filter from the XML file.
    /// </summary>
    /// <param name="filter">The filter function to apply.</param>
    /// <returns>The assignment that matches the filter, or null if none match.</returns>
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        return assignments.FirstOrDefault(filter);
    }

    /// <summary>
    /// Reads all assignments, optionally filtering them, from the XML file.
    /// </summary>
    /// <param name="filter">Optional filter function.</param>
    /// <returns>All assignments matching the filter, or all assignments if no filter is provided.</returns>
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        return filter == null ? assignments : assignments.Where(filter);
    }

    /// <summary>
    /// Updates an existing assignment in the XML file.
    /// </summary>
    /// <param name="item">The updated assignment.</param>
    public void Update(Assignment item)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);

        Assignment? existingAssignment = assignments.FirstOrDefault(a => a.Id == item.Id);
        if (existingAssignment == null)
            throw new DalDoesNotExistException($"Assignment with ID={item.Id} does not exist.");

        assignments.Remove(existingAssignment);
        assignments.Add(item);
        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignments_xml);
    }
}
