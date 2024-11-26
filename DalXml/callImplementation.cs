namespace Dal;

using System;
using System.Collections.Generic;
using DalApi;
using DO;

internal class callImplementation : ICall
{
    public void Create(Call item)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public void DeleteAll()
    {
        throw new NotImplementedException();
    }

    public Call Read(int id)
    {
        throw new NotImplementedException();
    }

    public Call? Read(Func<Call, bool> filter)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        throw new NotImplementedException();
    }

    public void Update(Call item)
    {
        throw new NotImplementedException();
    }
}
