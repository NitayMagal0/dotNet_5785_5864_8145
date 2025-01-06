using BO;

namespace BlApi;

public interface IVolunteer : IObservable
{
    Role SignIn(string name, string password);
    public IEnumerable<BO.VolunteerInList> GetVolunteersByCallType(BO.CallType callType);

    IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive, BO.CallType? VolunteerInList);//need to check about the enum
    Volunteer GetVolunteerDetails(int id);
    void UpdateVolunteer(int id, BO.Volunteer vol);
    void DeleteVolunteer(int id);
    void AddVolunteer(BO.Volunteer vol);
    int GetIdByName(string name);

}

