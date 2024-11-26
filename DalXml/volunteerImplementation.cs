namespace Dal;

using System;
using System.Collections.Generic;
using DalApi;
using DO;

internal class volunteerImplementation : IVolunteer
{ 

    /// <summary>
    /// Adds a new volunteer to the XML file.
    /// </summary>
    /// <param name="item">The Volunteer object to add.</param>
    public void Create(Volunteer item)
    {
        List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);

        if (volunteers.Any(v => v.Id == item.Id))
            throw new DalEntityAlreadyExistsException($"Volunteer with ID={item.Id} already exists");

        volunteers.Add(item);
        XMLTools.SaveListToXMLSerializer(volunteers, Config.s_volunteers_xml);
    }

    /// <summary>
    /// Deletes a volunteer by their ID from the XML file.
    /// </summary>
    /// <param name="id">The ID of the volunteer to delete.</param>
    public void Delete(int id)
    {
        List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);

        Volunteer? volunteer = volunteers.FirstOrDefault(v => v.Id == id);
        if (volunteer == null)
            throw new DalDoesNotExistException($"Volunteer with ID={id} does not exist");

        volunteers.Remove(volunteer);
        XMLTools.SaveListToXMLSerializer(volunteers, Config.s_volunteers_xml);
    }

    /// <summary>
    /// Deletes all volunteers from the XML file.
    /// </summary>
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Volunteer>(), Config.s_volunteers_xml);
    }

    /// <summary>
    /// Retrieves a volunteer by their ID from the XML file.
    /// </summary>
    /// <param name="id">The ID of the volunteer to retrieve.</param>
    /// <returns>The volunteer with the specified ID, or null if no such volunteer exists.</returns>
    public Volunteer Read(int id) 
    {
        List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        return volunteers.FirstOrDefault(v => v.Id == id)
               ?? throw new DalDoesNotExistException($"Volunteer with ID={id} does not exist");
    }

    /// <summary>
    /// Retrieves a volunteer by a specified filter from the XML file.
    /// </summary>
    /// <param name="filter">The filter to apply to find the volunteer.</param>
    /// <returns>The volunteer that matches the filter or null if no volunteer matches.</returns>
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        return volunteers.FirstOrDefault(filter);
    }

    /// <summary>
    /// Retrieves all volunteers from the XML file.
    /// </summary>
    /// <param name="filter">Optional filter to apply when retrieving volunteers.</param>
    /// <returns>A list of all volunteers matching the filter, or all volunteers if no filter is provided.</returns>
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
        return filter == null ? volunteers : volunteers.Where(filter);
    }


    /// <summary>
    /// Updates an existing volunteer in the XML file.
    /// </summary>
    /// <param name="item">The updated Volunteer object.</param>
    public void Update(Volunteer item)
    {
        List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);

        Volunteer? existingVolunteer = volunteers.FirstOrDefault(v => v.Id == item.Id);
        if (existingVolunteer == null)
            throw new DalDoesNotExistException($"Volunteer with ID={item.Id} does not exist");

        volunteers.Remove(existingVolunteer);
        volunteers.Add(item);
        XMLTools.SaveListToXMLSerializer(volunteers, Config.s_volunteers_xml);
    }

}
