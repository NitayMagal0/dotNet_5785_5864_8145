namespace Dal;

using System;
using System.Collections.Generic;
using System.Xml.Linq;
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
    public Volunteer? Read(int id)
    {
        
        XElement? volunteerElem =
    XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().FirstOrDefault(st => (int?)st.Element("Id") == id);
        return volunteerElem is null ? null : getVolunteer(volunteerElem);
    }

    static Volunteer getVolunteer(XElement v)
    {
        return new Volunteer()
        {
            Id = v.ToIntNullable("Id") ?? throw new FormatException("Can't convert Id"),
            FullName = (string?)v.Element("FullName") ?? "",
            MobilePhone = (string?)v.Element("MobilePhone") ?? "",
            Email = (string?)v.Element("Email") ?? "",
            Password = (string?)v.Element("Password"),
            FullAddress = (string?)v.Element("FullAddress"),
            Latitude = v.ToDoubleNullable("Latitude"),
            Longitude = v.ToDoubleNullable("Longitude"),
            Role = v.ToEnumNullable<Role>("Role") ?? Role.Volunteer,
            IsActive = (bool?)v.Element("IsActive") ?? false,
            MaxDistanceForCall = v.ToDoubleNullable("MaxDistanceForCall"),
            DistanceType = v.ToEnumNullable<DistanceType>("DistanceType") ?? DistanceType.AirDistance
        };
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
        // Load the root XML element containing all volunteers
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        // Find the volunteer with the matching ID or throw an exception if it doesn't exist
        XElement volunteerElem = volunteersRootElem.Elements().FirstOrDefault(v => (int?)v.Element("Id") == item.Id)
            ?? throw new DO.DalDoesNotExistException($"Volunteer with ID={item.Id} does not exist");

        // Update the existing volunteer element
        volunteerElem.SetElementValue("FullName", item.FullName);
        volunteerElem.SetElementValue("MobilePhone", item.MobilePhone);
        volunteerElem.SetElementValue("Email", item.Email);
        volunteerElem.SetElementValue("Password", item.Password);
        volunteerElem.SetElementValue("FullAddress", item.FullAddress);
        volunteerElem.SetElementValue("Latitude", item.Latitude);
        volunteerElem.SetElementValue("Longitude", item.Longitude);
        volunteerElem.SetElementValue("Role", item.Role);
        volunteerElem.SetElementValue("IsActive", item.IsActive);
        volunteerElem.SetElementValue("MaxDistanceForCall", item.MaxDistanceForCall);
        volunteerElem.SetElementValue("DistanceType", item.DistanceType);

        // Save the updated XML back to the file
        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteers_xml);
    }
    private XElement createVolunteerElement(Volunteer item)
    {
        return new XElement("Volunteer",
            new XElement("Id", item.Id),
            new XElement("FullName", item.FullName),
            new XElement("MobilePhone", item.MobilePhone),
            new XElement("Email", item.Email),
            new XElement("Password", item.Password),
            new XElement("FullAddress", item.FullAddress),
            new XElement("Latitude", item.Latitude),
            new XElement("Longitude", item.Longitude),
            new XElement("Role", item.Role),
            new XElement("IsActive", item.IsActive),
            new XElement("MaxDistanceForCall", item.MaxDistanceForCall),
            new XElement("DistanceType", item.DistanceType)
        );
    }


}
