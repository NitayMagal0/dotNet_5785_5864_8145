using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BO; // Business objects namespace

namespace PL.Volunteer
{
    public partial class CallHistory : Window
    {
        private readonly int _volunteerId;
        private readonly Func<int, CallType?, Enum?, IEnumerable<ClosedCallInList>> _getCallHistoryMethod;

        public CallHistory(int volunteerId, Func<int, CallType?, Enum?, IEnumerable<ClosedCallInList>> getCallHistoryMethod)
        {
            InitializeComponent();

            _volunteerId = volunteerId;
            _getCallHistoryMethod = getCallHistoryMethod;

            // Load filter options and call history
            LoadCallTypeFilter();
            LoadCallHistory();
        }

        private void LoadCallTypeFilter()
        {
            // Populate the ComboBox with CallType enum values
            CallTypeFilter.ItemsSource = Enum.GetValues(typeof(CallType)).Cast<CallType>();
            CallTypeFilter.SelectedIndex = -1; // No filter by default
        }

        private void LoadCallHistory(CallType? selectedCallType = null)
        {
            // Fetch the data from the logic layer
            var callHistory = _getCallHistoryMethod(_volunteerId, selectedCallType, null);

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

