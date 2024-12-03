using DalApi;
namespace Dal;

    sealed internal class DalXml : IDal
    {
        public static IDal Instance { get; } = new DalXml();
        private DalXml() { }

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

