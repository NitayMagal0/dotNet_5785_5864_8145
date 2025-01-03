using System.Windows;

namespace PL.Volunteer;

/// <summary>
/// Interaction logic for VolunteerWindow.xaml
/// </summary>
public partial class VolunteerWindow : Window
{
    /// <summary>
    /// Static instance of the business logic interface.
    /// </summary>
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    /// <summary>
    /// Gets or sets the selected role for the ComboBox.
    /// </summary>
    public BO.Role SelectedRole { get; set; } = BO.Role.Volunteer;

    /// <summary>
    /// Gets or sets the selected distance type for the ComboBox.
    /// </summary>
    public BO.DistanceType SelectedDistanceType { get; set; } = BO.DistanceType.AirDistance;

    /// <summary>
    /// Identifies the ButtonText dependency property.
    /// </summary>
    public static readonly DependencyProperty ButtonTextProperty =
        DependencyProperty.Register("ButtonText", typeof(string), typeof(VolunteerWindow));

    /// <summary>
    /// Gets or sets the text for the button.
    /// </summary>
    public string ButtonText
    {
        get { return (string)GetValue(ButtonTextProperty); }
        set { SetValue(ButtonTextProperty, value); }
    }

    /// <summary>
    /// Gets or sets the current volunteer.
    /// </summary>
    public BO.Volunteer? CurrentVolunteer
    {
        get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
        set { SetValue(CurrentVolunteerProperty, value); }
    }

    /// <summary>
    /// Identifies the CurrentVolunteer dependency property.
    /// </summary>
    public static readonly DependencyProperty CurrentVolunteerProperty =
        DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

    /// <summary>
    /// Initializes a new instance of the VolunteerWindow class.
    /// </summary>
    /// <param name="id">The ID of the volunteer. If 0, a new volunteer is being added.</param>
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

    /// <summary>
    /// Method to reload the current volunteer.
    /// </summary>
    private void ReloadVolunteer()
    {
        if (CurrentVolunteer != null)
        {
            int id = CurrentVolunteer.Id;
            CurrentVolunteer = null;
            CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
        }
    }

    /// <summary>
    /// Event handler for the Loaded event of the window.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event data for the event.</param>
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (CurrentVolunteer != null && CurrentVolunteer.Id != 0)
        {
            s_bl.Volunteer.AddObserver(CurrentVolunteer.Id, ReloadVolunteer);
        }
    }

    /// <summary>
    /// Event handler for the Unloaded event of the window.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event data for the event.</param>
    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (CurrentVolunteer != null && CurrentVolunteer.Id != 0)
        {
            s_bl.Volunteer.RemoveObserver(CurrentVolunteer.Id, ReloadVolunteer);
        }
    }

    /// <summary>
    /// Click event handler for the Add/Update button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event data for the event.</param>
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

