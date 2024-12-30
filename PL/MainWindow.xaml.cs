using System.Windows;
using System.Windows.Input;

namespace PL;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public MainWindow()
    {
        InitializeComponent();

        // Register the OnLoaded event handler for the Loaded event
        this.Loaded += MainWindow_OnLoaded;

        // Register the OnClosed event handler for the Closed event
        this.Closed += MainWindow_OnClosed;
    }

    // Event handler for the Loaded event
    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        // Fetch and set the current system clock value
        CurrentTime = s_bl.Admin.GetClock();

        // Fetch and set the MaxRange configuration variable
        MaxRange = s_bl.Admin.GetMaxRange();

        // Register observers for clock and configuration updates
        s_bl.Admin.AddClockObserver(clockObserver);
        s_bl.Admin.AddConfigObserver(configObserver);
    }

    // Event handler for the Closed event
    private void MainWindow_OnClosed(object sender, EventArgs e)
    {
        // Remove observers for clock and configuration updates
        s_bl.Admin.RemoveClockObserver(clockObserver);
        s_bl.Admin.RemoveConfigObserver(configObserver);
    }

    // Observer for clock updates
    private void clockObserver()
    {
        // Update the CurrentTime dependency property
        CurrentTime = s_bl.Admin.GetClock();
    }

    // Observer for configuration updates
    private void configObserver()
    {
        // Update the MaxRange dependency property
        MaxRange = s_bl.Admin.GetMaxRange();
    }
    public DateTime CurrentTime
    {
        get { return (DateTime)GetValue(CurrentTimeProperty); }
        set { SetValue(CurrentTimeProperty, value); }
    }
    public static readonly DependencyProperty CurrentTimeProperty = DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));
    public TimeSpan MaxRange
    {
        get { return (TimeSpan)GetValue(MaxRangeProperty); }
        set { SetValue(MaxRangeProperty, value); }
    }
    public static readonly DependencyProperty MaxRangeProperty = DependencyProperty.Register("MaxRange", typeof(TimeSpan), typeof(MainWindow));
    private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(BO.TimeUnit.minute);
    }
    private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(BO.TimeUnit.hour);
    }
    private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(BO.TimeUnit.day);
    }
    private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(BO.TimeUnit.month);
    }
    private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(BO.TimeUnit.year);
    }
    private void btnUpdateMaxRange_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Parse the MaxRange value from the TextBox
            if (TimeSpan.TryParse(txtMaxRange.Text, out TimeSpan maxRange))
            {
                // Call the method in the BL to update the configuration variable
                s_bl.Admin.SetMaxRange(maxRange);

                // Optionally provide feedback to the user
                MessageBox.Show("Max Range updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Show an error if the input is invalid
                MessageBox.Show("Invalid Max Range format. Please enter a valid TimeSpan value.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            // Handle any exceptions that occur during the update
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private void btnVolunteerList_Click(object sender, RoutedEventArgs e)
    {
        // Open the Volunteer List screen
        new Volunteer.VolunteerListWindow().Show();
    }
    private void btnResetDB_Click(object sender, RoutedEventArgs e)
    {
        if (MessageBox.Show("Are you sure you want to reset the database? This action cannot be undone.",
                "Confirm Reset",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.Yes)
        {
            PerformDatabaseOperation(() => s_bl.Admin.ResetDB(), "Database reset successfully.");
        }
    }
    private void btnInitializeDB_Click(object sender, RoutedEventArgs e)
    {
        if (MessageBox.Show("Are you sure you want to initialize the database? This action will overwrite existing data.",
                "Confirm Initialization",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.Yes)
        {
            PerformDatabaseOperation(() => s_bl.Admin.InitializeDB(), "Database initialized successfully.");
        }
    }

    private void PerformDatabaseOperation(Action dbOperation, string successMessage)
    {
        try
        {
            // Change the mouse cursor to hourglass
            Mouse.OverrideCursor = Cursors.Wait;

            // Close all open windows except the main window
            CloseAllOtherWindows();

            // Perform the database operation
            dbOperation.Invoke();

            // Notify the user of success
            MessageBox.Show(successMessage, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            // Handle any errors
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            // Restore the mouse cursor to default
            Mouse.OverrideCursor = null;
        }
    }

    private void CloseAllOtherWindows()
    {
        foreach (Window window in Application.Current.Windows)
        {
            if (window != this) // Keep the main window open
            {
                window.Close();
            }
        }
    }
}
