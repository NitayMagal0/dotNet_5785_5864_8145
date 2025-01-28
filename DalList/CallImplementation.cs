namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <summary>
/// The implementation of the interface ICall
/// </summary>
internal class CallImplementation : ICall
{
    /// <summary>
    /// This method creates a new item and updates the database
    /// </summary>
    /// <param name="item">The new item</param>
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void Create(Call item)
    {
        Call newCall = item with { Id = Config.NextCallId };
        DataSource.Calls.Add(newCall);
    }
    /// <summary>
    /// This function performs the deletion operation if the item exists otherwise throws an error
    /// </summary>
    /// <param name="id">The Id of the item to be deleted</param>
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void Delete(int id)
    {
        Call? call = Read(id);
        if (call is not null)   // Makes sure the item to delete exists
        {
            DataSource.Calls.Remove(call);
        }
        else
        {
            throw new DalDoesNotExistException($"Object of type Call with ID {id} does not exist.");
        }
    }
    // <summary>
    /// This function deletes the entire buffer by running the remove function on each of the items
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void DeleteAll()
    {
        DataSource.Calls.Clear();
        /*
        foreach (Call item in DataSource.Calls)
        {
            Delete(item.Id);
        }*/
    }
    /// <summary>
    /// The method looks for the object in the database if it finds it it returns it otherwise it returns null
    /// </summary>
    /// <param name="id">The ID number of the item you want to read</param>
    /// <returns>the requested item or Null</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]

    public Call? Read(int id)
    {
        return DataSource.Calls.FirstOrDefault(x => x.Id == id);
    }
    /// <summary>
    /// Returns a copy of the original list
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]

    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        return filter == null ? DataSource.Calls : DataSource.Calls.Where(filter);
    }
    /// <summary>
    /// This function is responsible for updating the database if the item to be updated exists, if it does not exist it will throw an error
    /// </summary>
    /// <param name="item">The item you want to update</param>
    [MethodImpl(MethodImplOptions.Synchronized)]

    public void Update(Call item)
    {
        Call? unupdatedCall = Read(item.Id);
        if (unupdatedCall is not null)
        {
            Delete(unupdatedCall!.Id);
            Call newCall = item with { Id = item.Id };
            DataSource.Calls.Add(newCall);
        }
        else                                       // if unupdatedCall is null it means the item doe's not exists
        {
            throw new DalDoesNotExistException($"Object of type Call with ID {item.Id} does not exist.");
        }
    }
    /// <summary>
    /// The method looks for the object in the database if it finds it, it returns it otherwise it returns null
    /// </summary>
    /// <param name="filter">The filter to apply to the items</param>
    /// <returns>the requested item or Null</returns>
    [MethodImpl(MethodImplOptions.Synchronized)]

    public Call? Read(Func<Call, bool> filter)
    {
        return DataSource.Calls.FirstOrDefault(filter);
    }
}
