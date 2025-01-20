using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using DO;

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
            var assignments = s_bl.Call.GetFilteredAndSortedCalls(null, CurrentVolunteer.Id, null);
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
            MessageBox.Show("Call History logic not implemented.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
