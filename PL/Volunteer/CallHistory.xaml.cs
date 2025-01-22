using System;
using System.Linq;
using System.Windows;
using BO; // Business objects namespace

namespace PL.Volunteer
{
    public partial class CallHistory : Window
    {
        private static readonly BlApi.IBl s_bl = BlApi.Factory.Get(); // Access to the business logic layer
        private readonly int _volunteerId;

        public CallHistory(int volunteerId)
        {
            InitializeComponent();

            _volunteerId = volunteerId;

            // Load filter options and call history
            LoadCallTypeFilter();
            LoadCallHistory();
        }

        private void LoadCallTypeFilter()
        {
            // Populate the ComboBox with CallType enum values
            CallTypeFilter.ItemsSource = Enum.GetValues(typeof(CallType)).Cast<CallType>();
            CallTypeFilter.SelectedIndex = 0; // Default to Undefined
        }

        private void LoadCallHistory(CallType? selectedCallType = null)
        {
            // If the selected call type is Undefined or not selected, set it to null
            if (selectedCallType == null || selectedCallType == CallType.Undefined)
            {
                selectedCallType = null;
            }

            // Fetch the data from the logic layer
            var callHistory = s_bl.Call.GetVolunteerClosedCallsHistory(_volunteerId, selectedCallType, null);

            // Bind the data to the DataGrid
            CallHistoryGrid.ItemsSource = callHistory.ToList();
        }

        private void CallTypeFilter_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Get the selected call type from the ComboBox
            var selectedCallType = CallTypeFilter.SelectedItem as CallType?;

            // Reload the call history with the selected filter
            LoadCallHistory(selectedCallType);
        }
    }
}