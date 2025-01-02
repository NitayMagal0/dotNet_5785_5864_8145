using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PL.Volunteer;

/// <summary>
/// Interaction logic for VolunteerListWindow.xaml
/// </summary>
public partial class VolunteerListWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public BO.VolunteerInList? SelectedVolunteer { get; set; }

    public VolunteerListWindow()
    {
        InitializeComponent();
        Loaded += Window_Loaded;
        Closed += Window_Closed;
    }
    public IEnumerable<BO.VolunteerInList> VolunteerList
    {
        get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
        set { SetValue(VolunteerListProperty, value); }
    }

    public static readonly DependencyProperty VolunteerListProperty =
        DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

    public BO.CallType searchFilter { get; set; } = BO.CallType.Undefined;

    // Event handler for SelectionChanged
    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        queryVolunteerList();
    }

    private void queryVolunteerList()
    {
        VolunteerList = (searchFilter == BO.CallType.Undefined)
            ? s_bl?.Volunteer.GetVolunteersList(null, null)
            : s_bl?.Volunteer.GetVolunteersByCallType(searchFilter);
    }
    private void volunteerListObserver()
    {
        queryVolunteerList();
    }
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        // Register the observer for changes in BL
        s_bl?.Volunteer.AddObserver(volunteerListObserver);
        // Initial population of the list
        queryVolunteerList();
    }

    private void Window_Closed(object sender, System.EventArgs e)
    {
        // Unregister the observer for changes in BL
        s_bl?.Volunteer.RemoveObserver(volunteerListObserver);
    }

    // Method to handle double-click on a volunteer in the list
    private void VolunteerList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SelectedVolunteer != null)
        {
            new VolunteerWindow(SelectedVolunteer.Id).Show();
        }
    }
    // Method to handle Add button click
    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        new VolunteerWindow().Show();
    }

    private void DeleteVolunteerButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is BO.VolunteerInList volunteer)
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

