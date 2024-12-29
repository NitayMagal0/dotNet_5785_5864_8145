using BO;

namespace BlApi;

public interface IVolunteer : IObservable
{
    Role SignIn(string name, string password);

    IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive, Enum? VolunteerInList);//need to check about the enum
    Volunteer GetVolunteerDetails(int id);
    void UpdateVolunteer(int id, BO.Volunteer vol);
    void DeleteVolunteer(int id);
    void AddVolunteer(BO.Volunteer vol);
}

