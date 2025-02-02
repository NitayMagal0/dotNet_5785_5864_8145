using System.Runtime.CompilerServices;
namespace Dal;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using DalApi;
using DO;
/// <summary>
/// Implementation of the ICall interface for managing Call entities using XML storage.
/// </summary>
internal class callImplementation : ICall
{
    /// <summary>
    /// Creates a new Call entity and adds it to the XML storage.
    /// </summary>
    /// <param name="item">The Call entity to be created.</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Call item)   //need to use config to increase the id of call
    {
        XElement callsRootElem = XMLTools.LoadListFromXMLElement(Config.s_calls_xml);
        // Add the new call
        callsRootElem.Add(createCallElement(item));

        // Save the updated XML
        XMLTools.SaveListToXMLElement(callsRootElem, Config.s_calls_xml);
    }

    /// <summary>
    /// Deletes a Call entity from the XML storage by its ID.
    /// </summary>
    /// <param name="id">The ID of the Call entity to be deleted.</param>
    /// <exception cref="DalDoesNotExistException">Thrown when the Call entity with the specified ID does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        XElement callsRootElem = XMLTools.LoadListFromXMLElement(Config.s_calls_xml);

        // Find the call and remove it, or throw an exception if not found
        (callsRootElem.Elements().FirstOrDefault(c => (int?)c.Element("Id") == id)
            ?? throw new DalDoesNotExistException($"Call with ID={id} does not exist"))
            .Remove();

        // Save the updated XML
        XMLTools.SaveListToXMLElement(callsRootElem, Config.s_calls_xml);
    }

    /// <summary>
    /// Deletes all Call entities from the XML storage.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        // Overwrite the XML file with an empty root element
        XMLTools.SaveListToXMLElement(new XElement("Calls"), Config.s_calls_xml);
    }

    /// <summary>
    /// Reads a Call entity from the XML storage by its ID.
    /// </summary>
    /// <param name="id">The ID of the Call entity to be read.</param>
    /// <returns>The Call entity with the specified ID.</returns>
    /// <exception cref="DalDoesNotExistException">Thrown when the Call entity with the specified ID does not exist.</exception>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call Read(int id)
    {
        XElement callsRootElem = XMLTools.LoadListFromXMLElement(Config.s_calls_xml);

        // Find the call or throw an exception if not found
        XElement? callElem = callsRootElem.Elements().FirstOrDefault(c => (int?)c.Element("Id") == id);
        return callElem is null
            ? throw new DalDoesNotExistException($"Call with ID={id} does not exist")
            : getCall(callElem);
    }

    /// <summary>
    /// Reads a Call entity from the XML storage that matches the specified filter.
    /// </summary>
    /// <param name="filter">The filter function to apply.</param>
    /// <returns>The first Call entity that matches the filter, or null if no match is found.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(Func<Call, bool> filter)
    {
        IEnumerable<Call> calls = ReadAll();
        return calls.FirstOrDefault(filter);
    }

    /// <summary>
    /// Reads all Call entities from the XML storage, optionally applying a filter.
    /// </summary>
    /// <param name="filter">The filter function to apply, or null to read all entities.</param>
    /// <returns>An enumerable collection of Call entities.</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        XElement callsRootElem = XMLTools.LoadListFromXMLElement(Config.s_calls_xml);

        // Convert all XML elements to Call objects and filter if needed
        IEnumerable<Call> calls = callsRootElem.Elements().Select(getCall);
        return filter == null ? calls : calls.Where(filter);
    }

    /// <summary>
    /// Updates an existing Call entity in the XML storage.
    /// </summary>
    /// <param name="item">The Call entity with updated values.</param>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Call item)
    {
        XElement callsRootElem = XMLTools.LoadListFromXMLElement(Config.s_calls_xml);

        // Find the call and remove it; return if not found
        XElement? existingCallElement = callsRootElem.Elements().FirstOrDefault(c => (int?)c.Element("Id") == item.Id);
        if (existingCallElement == null)
        {
            return; // Call with the given ID does not exist, exit the method
        }

        existingCallElement.Remove();

        callsRootElem.Add(
            new XElement("Call",
                new XElement("Id", item.Id),
                new XElement("CallType", item.CallType),
                new XElement("Description", item.Description),
                new XElement("FullAddress", item.FullAddress),
                new XElement("Latitude", item.Latitude),
                new XElement("Longitude", item.Longitude),
                new XElement("OpeningTime", item.OpeningTime),
                new XElement("MaxCompletionTime", item.MaxCompletionTime))
            );

        // Save the updated XML
        XMLTools.SaveListToXMLElement(callsRootElem, Config.s_calls_xml);
    }

    /// <summary>
    /// Creates an XElement representation of a Call entity.
    /// </summary>
    /// <param name="item">The Call entity to be converted.</param>
    /// <returns>An XElement representing the Call entity.</returns>
    private XElement createCallElement(Call item)
    {
        // Fetch and increment the next available ID
        int newId = Config.NextCallId;
        // Create a new Call object with the incremented ID
        return new XElement("Call",
            new XElement("Id", newId),
            new XElement("CallType", item.CallType),
            new XElement("Description", item.Description),
            new XElement("FullAddress", item.FullAddress),
            new XElement("Latitude", item.Latitude),
            new XElement("Longitude", item.Longitude),
            new XElement("OpeningTime", item.OpeningTime),
            new XElement("MaxCompletionTime", item.MaxCompletionTime)
        );
    }

    /// <summary>
    /// Converts an XElement to a Call entity.
    /// </summary>
    /// <param name="c">The XElement to be converted.</param>
    /// <returns>The Call entity represented by the XElement.</returns>
    /// <exception cref="FormatException">Thrown when the XElement cannot be converted to a Call entity.</exception>
    private Call getCall(XElement c)
    {
        return new Call
        {
            Id = c.ToIntNullable("Id") ?? throw new FormatException("Can't convert Id"),
            CallType = c.ToEnumNullable<CallType>("CallType") ?? CallType.Undefined,
            Description = (string?)c.Element("Description"),
            FullAddress = (string?)c.Element("FullAddress") ?? "",
            Latitude = c.ToDoubleNullable("Latitude") ?? 0.0,
            Longitude = c.ToDoubleNullable("Longitude") ?? 0.0,
            OpeningTime = c.ToDateTimeNullable("OpeningTime") ?? default,
            MaxCompletionTime = c.ToDateTimeNullable("MaxCompletionTime")
        };
    }
}
