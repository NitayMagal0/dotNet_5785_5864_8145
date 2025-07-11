﻿namespace Dal;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using DalApi;
using DO;

internal class assignmentImplementation : IAssignment
{

    /// <summary>
    /// Creates a new assignment and saves it in the XML file.
    /// </summary>
    /// <param name="item">The assignment to create.</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Assignment item)
    {
        XElement assignmentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
        /*    // Check if the call already exists
            if (callsRootElem.Elements().Any(c => (int?)c.Element("Id") == item.Id))
                throw new DalEntityAlreadyExistsException($"Call with ID={item.Id} already exists");*/
        // Add the new call
        assignmentsRootElem.Add(createAssignmentElement(item));
        XMLTools.SaveListToXMLElement(assignmentsRootElem, Config.s_assignments_xml);
    }

    /// <summary>
    /// Deletes an assignment by ID from the XML file.
    /// </summary>
    /// <param name="id">The ID of the assignment to delete.</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
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
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_assignments_xml);
    }

    /// <summary>
    /// Reads an assignment by ID from the XML file.
    /// </summary>
    /// <param name="id">The ID of the assignment to read.</param>
    /// <returns>The assignment with the specified ID.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]

    public Assignment Read(int id)
    {
        XElement assignmentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);

        // Find the call or throw an exception if not found
        XElement? assignmentElem = assignmentsRootElem.Elements().FirstOrDefault(c => (int?)c.Element("Id") == id);
        return assignmentElem is null
            ? throw new DalDoesNotExistException($"Assignment with ID={id} does not exist")
            : getAssignmnet(assignmentElem);
    }

    /// <summary>
    /// Reads an assignment that matches the specified filter from the XML file.
    /// </summary>
    /// <param name="filter">The filter function to apply.</param>
    /// <returns>The assignment that matches the filter, or null if none match.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]

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
    [MethodImpl(MethodImplOptions.Synchronized)]

    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        return filter == null ? assignments : assignments.Where(filter);
    }

    /// <summary>
    /// Updates an existing assignment in the XML file.
    /// </summary>
    /// <param name="item">The updated assignment.</param>
    [MethodImpl(MethodImplOptions.Synchronized)]

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
    [MethodImpl(MethodImplOptions.Synchronized)]

    private XElement createAssignmentElement(Assignment item)
    {
        // Fetch and increment the next available Assignment ID
        int newId = Config.NextAssignmentId;

        // Create the XElement for the assignment
        return new XElement("Assignment",
            new XElement("Id", newId),
            new XElement("CallId", item.CallId),
            new XElement("VolunteerId", item.VolunteerId),
            new XElement("AdmissionTime", item.AdmissionTime)
        //new XElement("ActualEndTime", item.ActualEndTime),
        //new XElement("AssignmentStatus", item.AssignmentStatus)
        );
    }

    private Assignment getAssignmnet(XElement a)
    {
        return new Assignment
        {
            Id = a.ToIntNullable("Id") ?? throw new FormatException("Can't convert Id"),
            CallId = a.ToIntNullable("CallId") ?? throw new FormatException("Can't convert CallId"),
            VolunteerId = a.ToIntNullable("VolunteerId") ?? throw new FormatException("Can't convert VolunteerId"),
            AdmissionTime = a.ToDateTimeNullable("AdmissionTime") ?? default,
            ActualEndTime = a.ToDateTimeNullable("ActualEndTime"),
            AssignmentStatus = a.ToEnumNullable<AssignmentStatus>("AssignmentStatus")
        };
    }

}

