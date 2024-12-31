using System.Windows;
using System.Windows.Controls;

namespace PL.Volunteer;

/// <summary>
/// Interaction logic for VolunteerListWindow.xaml
/// </summary>
public partial class VolunteerListWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
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

    public BO.CallType searchFilter { get; set; }  = BO.CallType.Undefined;

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
}

