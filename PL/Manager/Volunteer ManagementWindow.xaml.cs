using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PL.Volunteer;

/// <summary>
/// Interaction logic for VolunteerListWindow.xaml
/// </summary>
public partial class VolunteerListWindow : Window, INotifyCollectionChanged, INotifyPropertyChanged
{
    /// <summary>
    /// Static instance of the business logic interface.
    /// </summary>
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    /// <summary>
    /// Gets or sets the selected volunteer.
    /// </summary>
    public BO.VolunteerInList? SelectedVolunteer { get; set; }

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        CollectionChanged?.Invoke(this, e);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    /// <summary>
    /// Initializes a new instance of the VolunteerListWindow class.
    /// </summary>
    public VolunteerListWindow()
    {
        InitializeComponent();
        Loaded += Window_Loaded;
        Closed += Window_Closed;
    }

    /// <summary>
    /// Gets or sets the list of volunteers.
    /// </summary>
    public IEnumerable<BO.VolunteerInList> VolunteerList
    {
        get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
        set { SetValue(VolunteerListProperty, value); }
    }

    /// <summary>
    /// Identifies the VolunteerList dependency property.
    /// </summary>
    public static readonly DependencyProperty VolunteerListProperty =
        DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the search filter for volunteers.
    /// </summary>
    public BO.CallType searchFilter { get; set; } = BO.CallType.Undefined;

    /// <summary>
    /// Event handler for SelectionChanged event of the ComboBox.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event data for the event.</param>
    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        queryVolunteerList();
    }

    /// <summary>
    /// Queries the volunteer list based on the search filter.
    /// </summary>
    private void queryVolunteerList()
    {
        VolunteerList = (searchFilter == BO.CallType.Undefined)
            ? s_bl?.Volunteer.GetVolunteersList(null, null)
            : s_bl?.Volunteer.GetVolunteersByCallType(searchFilter);
    }

    /// <summary>
    /// Observer method to update the volunteer list.
    /// </summary>
    private void volunteerListObserver()
    {
        queryVolunteerList();
    }

    /// <summary>
    /// Event handler for the Loaded event of the window.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event data for the event.</param>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        // Register the observer for changes in BL
        s_bl?.Volunteer.AddObserver(volunteerListObserver);
        // Initial population of the list
        queryVolunteerList();
    }

    /// <summary>
    /// Event handler for the Closed event of the window.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event data for the event.</param>
    private void Window_Closed(object sender, System.EventArgs e)
    {
        // Unregister the observer for changes in BL
        s_bl?.Volunteer.RemoveObserver(volunteerListObserver);
    }

    /// <summary>
    /// Method to handle double-click on a volunteer in the list.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event data for the event.</param>
    private void VolunteerList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SelectedVolunteer != null)
        {
            new VolunteerWindow(SelectedVolunteer.Id).Show();
        }
    }

    /// <summary>
    /// Method to handle Add button click.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event data for the event.</param>
    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        new VolunteerWindow().Show();
    }

    /// <summary>
    /// Method to handle Delete button click for a volunteer.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event data for the event.</param>
    private void DeleteVolunteerButton_Click(object sender, RoutedEventArgs e)
    {
        if ((e.OriginalSource as Button)?.CommandParameter is BO.VolunteerInList volunteer)
        {
            var result = MessageBox.Show($"Are you sure you want to delete {volunteer.FullName}?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Volunteer.DeleteVolunteer(volunteer.Id); // Call the existing BL method
                    MessageBox.Show($"{volunteer.FullName} has been successfully deleted.",
                        "Delete Successful",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete {volunteer.FullName}.\nError: {ex.Message}",
                        "Delete Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }
    }

}
