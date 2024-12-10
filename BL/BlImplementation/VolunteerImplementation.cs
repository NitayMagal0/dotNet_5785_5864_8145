namespace BlImplementation;
using BlApi;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void AddVolunteer(BO.Volunteer vol)
    {
        
    }

    public void DeleteVolunteer(int id)
    {
        throw new NotImplementedException();
    }

    public DO.Volunteer GetVolunteerDetails(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.Volunteer> GetVolunteersList(bool? isActive, Enum? VolunteerInList)
    {
        throw new NotImplementedException();
    }

    public DO.Role SignIn(string name, string password)
    {
        throw new NotImplementedException();
    }

    public void UpdateVolunteer(int id, BO.Volunteer vol)
    {
        throw new NotImplementedException();
    }
}

