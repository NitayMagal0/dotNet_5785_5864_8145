namespace Dal;

using System;
using System.Collections.Generic;
using DalApi;
using DO;

internal class callImplementation : ICall
{
    /// <summary>
    /// Creates a new call and saves it in the XML file.
    /// </summary>
    /// <param name="item">The call to create.</param>
    public void Create(Call item)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        Call newCall = item with { Id = XMLTools.GetAndIncreaseConfigIntVal("config.xml", "NextCallId") };
        calls.Add(newCall);
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml);
    }

    /// <summary>
    /// Deletes a call by ID from the XML file.
    /// </summary>
    /// <param name="id">The ID of the call to delete.</param>
    public void Delete(int id)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);

        Call? call = calls.FirstOrDefault(c => c.Id == id);
        if (call == null)
            throw new DalDoesNotExistException($"Call with ID={id} does not exist.");

        calls.Remove(call);
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml);
    }

    /// <summary>
    /// Deletes all calls from the XML file.
    /// </summary>
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Call>(), Config.s_calls_xml);
    }

    /// <summary>
    /// Reads a call by ID from the XML file.
    /// </summary>
    /// <param name="id">The ID of the call to read.</param>
    /// <returns>The call with the specified ID.</returns>
    public Call Read(int id)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return calls.FirstOrDefault(c => c.Id == id)
               ?? throw new DalDoesNotExistException($"Call with ID={id} does not exist.");
    }

    /// <summary>
    /// Reads a call that matches the specified filter from the XML file.
    /// </summary>
    /// <param name="filter">The filter function to apply.</param>
    /// <returns>The call that matches the filter, or null if none match.</returns>
    public Call? Read(Func<Call, bool> filter)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return calls.FirstOrDefault(filter);
    }

    /// <summary>
    /// Reads all calls, optionally filtering them, from the XML file.
    /// </summary>
    /// <param name="filter">Optional filter function.</param>
    /// <returns>All calls matching the filter, or all calls if no filter is provided.</returns>
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return filter == null ? calls : calls.Where(filter);
    }

    /// <summary>
    /// Updates an existing call in the XML file.
    /// </summary>
    /// <param name="item">The updated call.</param>
    public void Update(Call item)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);

        Call? existingCall = calls.FirstOrDefault(c => c.Id == item.Id);
        if (existingCall == null)
            throw new DalDoesNotExistException($"Call with ID={item.Id} does not exist.");

        calls.Remove(existingCall);
        calls.Add(item);
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml);
    }
}
