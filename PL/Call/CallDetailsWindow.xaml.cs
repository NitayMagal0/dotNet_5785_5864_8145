using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallDetailsWindow.xaml
    /// </summary>
    public partial class CallDetailsWindow : Window, INotifyPropertyChanged
    {
        // Static instance of the business logic interface
        private static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        private BO.Call? _currentCall;
        public BO.Call? CurrentCall
        {
            get => _currentCall;
            set
            {
                _currentCall = value;
                OnPropertyChanged(nameof(CurrentCall));
                OnPropertyChanged(nameof(IsCallEditable));
            }
        }
        private BO.CallInList? _currentCallInList;
        public BO.CallInList? CurrentCallInList
        {
            get => _currentCallInList;
            set
            {
                _currentCallInList = value;
                OnPropertyChanged(nameof(CurrentCallInList));
            }
        }

        /// <summary>
        /// Check if the call is editable by its status.
        /// </summary>
        public bool IsCallEditable
        {
            get => CurrentCall != null &&
                   (CurrentCall.Status == BO.CallStatus.Open ||
                    CurrentCall.Status == BO.CallStatus.OpenAtRisk);
        }

        public ObservableCollection<BO.CallType> CallTypes { get; set; }
        public ObservableCollection<BO.CallStatus> StatusTypes { get; set; }

        public CallDetailsWindow(int callId)
        {
            InitializeComponent();
            DataContext = this;

            LoadCallDetails(callId);
            LoadCallInList(callId);
            LoadCallTypes();
            LoadCallStatuses();
        }

        /// <summary>
        /// Load call details from the business logic layer.
        /// </summary>
        /// <param name="callId">ID of the call to be loaded.</param>
        private void LoadCallDetails(int callId)
        {
            try
            {
                CurrentCall = s_bl.Call.GetCallDetails(callId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load call details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        /// <summary>
        /// Load call in list details from the business logic layer.
        /// </summary>
        /// <param name="callId"></param>
        private void LoadCallInList(int callId)
        {
            try
            {
                CurrentCallInList = s_bl.Call.GetCallInListById(callId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load call summary details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Load available call types.
        /// </summary>
        private void LoadCallTypes()
        {
            CallTypes = new ObservableCollection<BO.CallType>((BO.CallType[])Enum.GetValues(typeof(BO.CallType)));
            OnPropertyChanged(nameof(CallTypes));
        }

        /// <summary>
        /// Load available call statuses.
        /// </summary>
        private void LoadCallStatuses()
        {
            StatusTypes = new ObservableCollection<BO.CallStatus>((BO.CallStatus[])Enum.GetValues(typeof(BO.CallStatus)));
            OnPropertyChanged(nameof(StatusTypes));
        }

        /// <summary>
        /// Handle the update button click event.
        /// </summary>
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentCall == null)
                {
                    MessageBox.Show("No call data to update.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                s_bl.Call.UpdateCall(CurrentCall);
                MessageBox.Show("Call updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handle the watch call assign list button click event.
        /// </summary>
        private void btnWatchCallAssignList_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentCall == null)
            {
                MessageBox.Show("No call selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Open the Call Assign List window
            // var assignListWindow = new CallAssignListWindow(CurrentCall.CallId);
            // assignListWindow.ShowDialog();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
