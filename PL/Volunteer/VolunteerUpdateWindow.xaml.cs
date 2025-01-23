using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace PL.Volunteer;

/// <summary>
/// Interaction logic for VolunteerUpdateWindow.xaml
/// </summary>
public partial class VolunteerUpdateWindow : Window, INotifyPropertyChanged
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public BO.Volunteer? CurrentVolunteer
    {
        get => (BO.Volunteer?)GetValue(CurrentVolunteerProperty);
        set => SetValue(CurrentVolunteerProperty, value);
    }

    public static readonly DependencyProperty CurrentVolunteerProperty =
        DependencyProperty.Register(
            "CurrentVolunteer",
            typeof(BO.Volunteer),
            typeof(VolunteerUpdateWindow),
            new PropertyMetadata(null, OnCurrentVolunteerChanged));

    private static void OnCurrentVolunteerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var window = d as VolunteerUpdateWindow;
        window?.LoadCalls();
    }

    private ObservableCollection<BO.Call> _calls = new();
    public ObservableCollection<BO.Call> Calls
    {
        get => _calls;
        set
        {
            _calls = value;
            OnPropertyChanged(nameof(Calls));
        }
    }

    public VolunteerUpdateWindow(int id)
    {
        InitializeComponent();
        DataContext = this;

        CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);

        LoadCalls();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void LoadCalls()
    {
        if (CurrentVolunteer != null)
        {
            // Assuming this returns a single call object, not a collection.
            var call = s_bl.Call.GetCallsForVolunteer(CurrentVolunteer.Id);

            // Handle single call or clear the collection if no call is found.
            Calls = call != null
                ? new ObservableCollection<BO.Call> { call } // Wrap single call in a collection.
                : new ObservableCollection<BO.Call>();
        }
    }


    private void ReloadVolunteer()
    {
        if (CurrentVolunteer != null)
        {
            int id = CurrentVolunteer.Id;
            CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
            LoadCalls();
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (CurrentVolunteer != null && CurrentVolunteer.Id != 0)
        {
            s_bl.Volunteer.AddObserver(CurrentVolunteer.Id, ReloadVolunteer);
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (CurrentVolunteer != null && CurrentVolunteer.Id != 0)
        {
            s_bl.Volunteer.RemoveObserver(CurrentVolunteer.Id, ReloadVolunteer);
        }
    }

    private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (CurrentVolunteer != null && !CurrentVolunteer.IsActive && Calls.Count > 0)
            {
                MessageBox.Show("The volunteer cannot be marked as inactive while they have active calls.",
                    "Validation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            s_bl.Volunteer.UpdateVolunteer(CurrentVolunteer.Id, CurrentVolunteer);
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
            if (CurrentVolunteer == null)
            {
                MessageBox.Show("No volunteer is selected.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var openCallsWindow = new OpenCalls(CurrentVolunteer.Id);
            openCallsWindow.Show();
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
            if (CurrentVolunteer == null)
            {
                MessageBox.Show("No volunteer is selected.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var callHistoryWindow = new CallHistory(CurrentVolunteer.Id);
            callHistoryWindow.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void EndTreatment_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (CurrentVolunteer == null)
            {
                MessageBox.Show("No volunteer is selected.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get single call for the volunteer
            var call = s_bl.Call.GetCallsForVolunteer(CurrentVolunteer.Id);
            if (call == null)
            {
                MessageBox.Show("No active treatment found for this volunteer.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            s_bl.Call.MarkAssignmentAsCompleted(CurrentVolunteer.Id, call.Id);
            MessageBox.Show("Treatment ended successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            ReloadVolunteer();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    private void CancelTreatment_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (CurrentVolunteer == null)
            {
                MessageBox.Show("No volunteer is selected.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get single call for the volunteer
            var call = s_bl.Call.GetCallsForVolunteer(CurrentVolunteer.Id);
            if (call == null)
            {
                MessageBox.Show("No active treatment found for this volunteer.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                "Are you sure you want to cancel this treatment?",
                "Confirm Cancellation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                s_bl.Call.CancelAssignment(CurrentVolunteer.Id, call.Id);
                MessageBox.Show("Treatment canceled successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                ReloadVolunteer();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

}
