using System.Windows;
using System.ComponentModel;

namespace PL.Volunteer;

/// <summary>
/// Interaction logic for VolunteerUpdateWindow.xaml
/// </summary>
public partial class VolunteerUpdateWindow : Window, INotifyPropertyChanged
{
    /// <summary>
    /// Static instance of the business logic interface.
    /// </summary>
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Gets whether the current volunteer is a manager.
    /// </summary>
    public bool IsManager => CurrentVolunteer?.Role == BO.Role.Manager;

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
        DependencyProperty.Register("ButtonText", typeof(string), typeof(VolunteerUpdateWindow));

    /// <summary>
    /// Gets or sets the text for the button.
    /// </summary>
    public string ButtonText
    {
        get => (string)GetValue(ButtonTextProperty);
        set => SetValue(ButtonTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the current volunteer.
    /// </summary>
    public BO.Volunteer? CurrentVolunteer
    {
        get => (BO.Volunteer?)GetValue(CurrentVolunteerProperty);
        set => SetValue(CurrentVolunteerProperty, value);
    }

    /// <summary>
    /// Identifies the CurrentVolunteer dependency property.
    /// Includes a PropertyChangedCallback for dynamic updates.
    /// </summary>
    public static readonly DependencyProperty CurrentVolunteerProperty =
        DependencyProperty.Register(
            "CurrentVolunteer",
            typeof(BO.Volunteer),
            typeof(VolunteerUpdateWindow),
            new PropertyMetadata(null, OnCurrentVolunteerChanged));

    private static void OnCurrentVolunteerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var window = d as VolunteerUpdateWindow;
        window?.OnPropertyChanged(nameof(IsManager));
    }

    /// <summary>
    /// Initializes a new instance of the VolunteerUpdateWindow class.
    /// </summary>
    /// <param name="id">The ID of the volunteer.</param>
    public VolunteerUpdateWindow(int id)
    {
        ButtonText = "Update";

        InitializeComponent();

        CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);

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
            CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
        }
    }

    /// <summary>
    /// Event handler for the Loaded event of the window.
    /// </summary>
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
    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (CurrentVolunteer != null && CurrentVolunteer.Id != 0)
        {
            s_bl.Volunteer.RemoveObserver(CurrentVolunteer.Id, ReloadVolunteer);
        }
    }

    /// <summary>
    /// Click event handler for the Update button.
    /// </summary>
    private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            s_bl.Volunteer.UpdateVolunteer(CurrentVolunteer!.Id, CurrentVolunteer!);
            MessageBox.Show("Volunteer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SelectCall_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Logic for selecting a call
            // Example placeholder code:
            MessageBox.Show("Select Call logic not implemented.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CallHistory_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Logic for displaying call history
            // Example placeholder code:
            MessageBox.Show("Call History logic not implemented.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

