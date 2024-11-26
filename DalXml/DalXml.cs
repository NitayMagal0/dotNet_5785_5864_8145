using DalApi;
namespace Dal;

    sealed class DalXml : IDal
    {
        public IAssignment Assignment { get; } = new assignmentImplementation();
        public ICall Call { get; } = new callImplementation();
        public IConfig Config { get; } = new ConfigImplementation();
        public IVolunteer Volunteer { get; } = new volunteerImplementation();

        public void ResetDB()
        {
            Volunteer.DeleteAll();
            Call.DeleteAll();
            Assignment.DeleteAll();
            Config.Reset();
        }
    }

