using System.Windows;
using BO;

namespace PL.Volunteer
{
    public partial class OpenCalls : Window
    {
        private static readonly BlApi.IBl s_bl = BlApi.Factory.Get(); // Access to the business logic layer
        private readonly int _volunteerId;

        public OpenCalls(int volunteerId)
        {
            InitializeComponent();

            _volunteerId = volunteerId;

            // Load filter options and open calls
            LoadCallTypeFilter();
            LoadOpenCalls();
        }

        private void LoadCallTypeFilter()
        {
            // Populate the ComboBox with CallType enum values
            CallTypeFilter.ItemsSource = Enum.GetValues(typeof(CallType)).Cast<CallType>();
            CallTypeFilter.SelectedIndex = -1; // No filter by default
        }

        private void LoadOpenCalls(CallType? selectedCallType = null)
        {
            // If the selected call type is Undefined or not selected, set it to null
            if (selectedCallType == null || selectedCallType == CallType.Undefined)
            {
                selectedCallType = null;
            }

            // Fetch open calls from the business logic layer
            var openCalls = s_bl.Call.GetAvailableOpenCallsForVolunteer(_volunteerId, selectedCallType, null);

            // Bind the data to the DataGrid
            OpenCallsGrid.ItemsSource = openCalls.ToList();
        }

        private void CallTypeFilter_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Get the selected call type from the ComboBox
            var selectedCallType = CallTypeFilter.SelectedItem as CallType?;

            // Reload the open calls with the selected filter
            LoadOpenCalls(selectedCallType);
        }
    }
}