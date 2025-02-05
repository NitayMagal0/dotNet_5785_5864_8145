using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;


namespace PL.Manager
{
    /// <summary>
    /// Interaction logic for ManageCallsWindow.xaml
    /// </summary>
    public partial class ManageCalls : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// Static instance of the business logic interface.
        /// </summary>
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        /// <summary>
        /// Gets or sets the selected call.
        /// </summary>
        public BO.CallInList? SelectedCall { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// Initializes a new instance of the ManageCallsWindow class.
        /// </summary>
        public ManageCalls()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
            Closed += Window_Closed;
        }


        /// <summary>
        /// Gets or sets the list of calls.
        /// </summary>
        public IEnumerable<BO.CallInList> CallInList
        {
            get { return (IEnumerable<BO.CallInList>)GetValue(CallInListProperty); }
            set { SetValue(CallInListProperty, value); }
        }

        /// <summary>
        /// Identifies the CallInList dependency property.
        /// </summary>
        public static readonly DependencyProperty CallInListProperty =
            DependencyProperty.Register("CallInList", typeof(IEnumerable<BO.CallInList>), typeof(ManageCalls), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the search filter for calls.
        /// </summary>
        public BO.CallStatus searchFilter
        {
            get { return (BO.CallStatus)GetValue(searchFilterProperty); }
            set { SetValue(searchFilterProperty, value); }
        }

        /// <summary>
        /// Identifies the searchFilter dependency property.
        /// </summary>
        public static readonly DependencyProperty searchFilterProperty =
            DependencyProperty.Register("searchFilter", typeof(BO.CallStatus), typeof(ManageCalls), new PropertyMetadata(BO.CallStatus.Open, OnSearchFilterChanged));

        /// <summary>
        /// Callback method that is called when the searchFilter property changes.
        /// </summary>
        /// <param name="d">The dependency object on which the property changed.</param>
        /// <param name="e">Event arguments that contain the old and new values of the property.</param>
        private static void OnSearchFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ManageCalls manageCallsWindow)
            {
                // Query the call list based on the new search filter value.
                manageCallsWindow.QueryCallList();
            }
        }

        //////
        /// <summary>
        /// Event handler for SelectionChanged event of the ComboBox.
        /// </summary>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            QueryCallList();
        }

        /// <summary>
        /// Queries the call list based on the search filter.
        /// </summary>
        private void QueryCallList()
        {
            CallInList = (searchFilter == BO.CallStatus.Open)
                ? s_bl?.Call.GetFilteredAndSortedCalls(null,null,null)
                : s_bl?.Call.GetFilteredAndSortedCalls(null, searchFilter, null);
        }


        private volatile DispatcherOperation? _observerOperation = null; //stage 7
        /// <summary>
        /// 
        /// method to update the call list.
        /// </summary>
        private void CallListObserver()
        { 
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    QueryCallList();
                });

        }

        /// <summary>
        /// Event handler for the Loaded event of the window.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Register the observer for changes in BL
            s_bl?.Call.AddObserver(CallListObserver);
            // Initial population of the list
            QueryCallList();
        }

        /// <summary>
        /// Event handler for the Closed event of the window.
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            // Unregister the observer for changes in BL
            s_bl?.Call.RemoveObserver(CallListObserver);
        }

        /// <summary>
        /// Method to handle double-click on a call in the list.
        /// </summary>
        private void CallList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCall != null)
            {
                if (SelectedCall.CallId > 0)  // Ensure ID is valid
                {
                    new Call.CallDetailsWindow(SelectedCall.CallId).Show();
                }
                else
                {
                    MessageBox.Show("Selected call ID is invalid.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("No call selected. Please select a call.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }



        /// <summary>
        /// Method to handle Add Call button click.
        /// </summary>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            new Call.AddCallWindow().Show();
        }

        /// <summary>
        /// Method to handle Delete button click for a call.
        /// </summary>
        private void DeleteCallButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is BO.CallInList call)
            {
                var result = MessageBox.Show($"Are you sure you want to delete this Call {call.CallId}?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        s_bl.Call.DeleteCall(call.CallId); // Call the existing BL method
                        MessageBox.Show($"Call ID {call.CallId} has been successfully deleted.",
                            "Delete Successful",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to delete Call ID {call.CallId}.\nError: {ex.Message}",
                            "Delete Failed",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
