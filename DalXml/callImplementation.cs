
using System.Runtime.CompilerServices;

namespace Dal;

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using DalApi;
using DO;

internal class callImplementation : ICall
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Call item)   //need to use config to increase the id of call
    {
        XElement callsRootElem = XMLTools.LoadListFromXMLElement(Config.s_calls_xml);
        /*    // Check if the call already exists
            if (callsRootElem.Elements().Any(c => (int?)c.Element("Id") == item.Id))
                throw new DalEntityAlreadyExistsException($"Call with ID={item.Id} already exists");*/
        // Add the new call
        callsRootElem.Add(createCallElement(item));

        // Save the updated XML
        XMLTools.SaveListToXMLElement(callsRootElem, Config.s_calls_xml);
    }

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

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        // Overwrite the XML file with an empty root element
        XMLTools.SaveListToXMLElement(new XElement("Calls"), Config.s_calls_xml);
    }

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

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(Func<Call, bool> filter)
    {
        IEnumerable<Call> calls = ReadAll();
        return calls.FirstOrDefault(filter);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        XElement callsRootElem = XMLTools.LoadListFromXMLElement(Config.s_calls_xml);

        // Convert all XML elements to Call objects and filter if needed
        IEnumerable<Call> calls = callsRootElem.Elements().Select(getCall);
        return filter == null ? calls : calls.Where(filter);
    }

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
