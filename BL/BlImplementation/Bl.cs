using BlApi;
namespace BlImplementation;

internal class Bl : IBl
{
    public IAdmin Admin { get; } = new AdminImplementation();
    public ICall Call { get; } = new CallImplementation();
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();

}

