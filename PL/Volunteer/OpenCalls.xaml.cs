using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PL.Volunteer;

/// <summary>
/// Interaction logic for OpenCalls.xaml
/// </summary>
public partial class OpenCalls : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public ObservableCollection<BO.Call> Calls { get; set; } = new();
    private ObservableCollection<BO.Call> _filteredCalls = new();

    public OpenCalls(int volunteerId)
    {
        InitializeComponent();
        DataContext = this;

        // Load calls for the volunteer
        LoadOpenCalls(volunteerId);
    }

    private void LoadOpenCalls(int volunteerId)
    {
        try
        {
            var openCalls = s_bl.Call.GetOpenCallsForVolunteer(volunteerId); // Replace with your actual method
            Calls = new ObservableCollection<BO.Call>(openCalls);
            _filteredCalls = new ObservableCollection<BO.Call>(Calls); // Clone for filtering
            OpenCallsDataGrid.ItemsSource = _filteredCalls;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void FilterButton_Click(object sender, RoutedEventArgs e)
    {
        var filterText = FilterTextBox.Text?.ToLower();

        if (!string.IsNullOrWhiteSpace(filterText))
        {
            _filteredCalls.Clear();

            foreach (var call in Calls.Where(c =>
                         c.CallType.ToString().ToLower().Contains(filterText) ||
                         (c.CallTypeDescription != null && c.CallTypeDescription.ToLower().Contains(filterText)) ||
                         (c.FullAddress != null && c.FullAddress.ToLower().Contains(filterText)) ||
                         c.Id.ToString().Contains(filterText)))
            {
                _filteredCalls.Add(call);
            }
        }
        else
        {
            MessageBox.Show("Please enter text to filter.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
    {
        FilterTextBox.Clear();
        _filteredCalls.Clear();

        foreach (var call in Calls)
        {
            _filteredCalls.Add(call);
        }
    }
}
