using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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

            try
            {
                // Retrieve the volunteer's maximum range
                var volunteer = s_bl.Volunteer.GetVolunteerDetails(_volunteerId);
                var maxRange = volunteer.MaxDistanceForCall; // Assume `MaximumRange` is a property in the volunteer's data
                // Define default distance type and sort field
                var distanceType = DistanceType.AirDistance;
                // Fetch calls based on whether a maximum range is defined
                var calls = maxRange.HasValue
                    ? s_bl.Call.GetNearbyOpenCallsForVolunteer(_volunteerId, maxRange.Value, distanceType, selectedCallType, null)
                    : s_bl.Call.GetAvailableOpenCallsForVolunteer(_volunteerId, selectedCallType, null);

                // Bind the data to the DataGrid
                OpenCallsGrid.ItemsSource = calls.ToList();
            }
            catch (Exception ex)
            {
                // Show error message
                MessageBox.Show($"Failed to load open calls.\nError: {ex.Message}",
                    "Load Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void CallTypeFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the selected call type from the ComboBox
            var selectedCallType = CallTypeFilter.SelectedItem as CallType?;

            // Reload the open calls with the selected filter
            LoadOpenCalls(selectedCallType);
        }

        private void AssignCallButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the call ID from the button's CommandParameter
            if ((sender as Button)?.CommandParameter is int callId)
            {
                try
                {
                    // Call the BL method to assign the call
                    s_bl.Call.AssignCallToVolunteer(_volunteerId, callId);

                    // Show success message
                    MessageBox.Show($"Call {callId} has been successfully assigned to the volunteer.",
                        "Assignment Successful",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    // Refresh the open calls list
                    LoadOpenCalls(CallTypeFilter.SelectedItem as CallType?);
                }
                catch (Exception ex)
                {
                    // Show error message
                    MessageBox.Show($"Failed to assign the call.\nError: {ex.Message}",
                        "Assignment Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }
    }
}