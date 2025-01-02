using System.Windows;

namespace PL.Volunteer;

/// <summary>
/// Interaction logic for VolunteerWindow.xaml
/// </summary>
public partial class VolunteerWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    // Selected values for ComboBox
    public BO.Role SelectedRole { get; set; } = BO.Role.Volunteer;
    public BO.DistanceType SelectedDistanceType { get; set; } = BO.DistanceType.AirDistance;

    // Dependency property for ButtonText
    public static readonly DependencyProperty ButtonTextProperty =
        DependencyProperty.Register("ButtonText", typeof(string), typeof(VolunteerWindow));

    public string ButtonText
    {
        get { return (string)GetValue(ButtonTextProperty); }
        set { SetValue(ButtonTextProperty, value); }
    }

    public BO.Volunteer? CurrentVolunteer
    {
        get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
        set { SetValue(CurrentVolunteerProperty, value); }
    }

    public static readonly DependencyProperty CurrentVolunteerProperty =
        DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

    public VolunteerWindow(int id = 0)
    {
        // Determine button text based on state
        ButtonText = id == 0 ? "Add" : "Update";

        InitializeComponent();
        // Set or retrieve CurrentVolunteer based on state
        CurrentVolunteer = id != 0
            ? s_bl.Volunteer.GetVolunteerDetails(id)
            : new BO.Volunteer();

        // Register events
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    // Method to reload the current volunteer
    private void ReloadVolunteer()
    {
        if (CurrentVolunteer != null)
        {
            int id = CurrentVolunteer.Id;
            CurrentVolunteer = null;
            CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
        }
    }

    // Screen load event handler
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (CurrentVolunteer != null && CurrentVolunteer.Id != 0)
        {
            s_bl.Volunteer.AddObserver(CurrentVolunteer.Id, ReloadVolunteer);
        }
    }

    // Screen close event handler
    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (CurrentVolunteer != null && CurrentVolunteer.Id != 0)
        {
            s_bl.Volunteer.RemoveObserver(CurrentVolunteer.Id, ReloadVolunteer);
        }
    }


    // Click event handler for the button
    private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (ButtonText == "Add")
            {
                s_bl.Volunteer.AddVolunteer(CurrentVolunteer!);
                MessageBox.Show("Volunteer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close(); // Close the window automatically
            }
            else
            {
                s_bl.Volunteer.UpdateVolunteer(CurrentVolunteer!.Id, CurrentVolunteer!);
                MessageBox.Show("Volunteer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close(); // Close the window automatically
            }
        }
        catch (Exception ex)
        {
            // Display the error message
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

