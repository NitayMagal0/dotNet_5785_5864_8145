using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO;

namespace BlApi
{
    public interface IVolunteer
    {
        Role SignIn(string name, string password);

        IEnumerable<BO.Volunteer> GetVolunteersList(bool? isActive, Enum? VolunteerInList);//need to check about the enum
        Volunteer GetVolunteerDetails(int id);
        void UpdateVolunteer(int id, BO.Volunteer vol);
        void DeleteVolunteer(int id);
        void AddVolunteer(BO.Volunteer vol);
    }
}
