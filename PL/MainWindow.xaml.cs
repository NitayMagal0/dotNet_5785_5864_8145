using System.Windows;
using System.Windows.Input;

namespace PL;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Static instance of the business logic interface.
    /// </summary>
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    /// <summary>
    /// Initializes a new instance of the MainWindow class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();

        // Register the OnLoaded event handler for the Loaded event
        this.Loaded += MainWindow_OnLoaded;

        // Register the OnClosed event handler for the Closed event
        this.Closed += MainWindow_OnClosed;
    }

    /// <summary>
    /// Event handler for the Loaded event.
    /// </summary>
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

    /// <summary>
    /// Event handler for the Closed event.
    /// </summary>
    private void MainWindow_OnClosed(object sender, EventArgs e)
    {
        // Remove observers for clock and configuration updates
        s_bl.Admin.RemoveClockObserver(clockObserver);
        s_bl.Admin.RemoveConfigObserver(configObserver);
    }

    /// <summary>
    /// Observer for clock updates.
    /// </summary>
    private void clockObserver()
    {
        // Update the CurrentTime dependency property
        CurrentTime = s_bl.Admin.GetClock();
    }

    /// <summary>
    /// Observer for configuration updates.
    /// </summary>
    private void configObserver()
    {
        // Update the MaxRange dependency property
        MaxRange = s_bl.Admin.GetMaxRange();
    }

    /// <summary>
    /// Gets or sets the current time.
    /// </summary>
    public DateTime CurrentTime
    {
        get { return (DateTime)GetValue(CurrentTimeProperty); }
        set { SetValue(CurrentTimeProperty, value); }
    }

    /// <summary>
    /// Identifies the CurrentTime dependency property.
    /// </summary>
    public static readonly DependencyProperty CurrentTimeProperty = DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));

    /// <summary>
    /// Gets or sets the maximum range.
    /// </summary>
    public TimeSpan MaxRange
    {
        get { return (TimeSpan)GetValue(MaxRangeProperty); }
        set { SetValue(MaxRangeProperty, value); }
    }

    /// <summary>
    /// Identifies the MaxRange dependency property.
    /// </summary>
    public static readonly DependencyProperty MaxRangeProperty = DependencyProperty.Register("MaxRange", typeof(TimeSpan), typeof(MainWindow));

    /// <summary>
    /// Click event handler for the Add One Minute button.
    /// </summary>
    private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(BO.TimeUnit.minute);
    }

    /// <summary>
    /// Click event handler for the Add One Hour button.
    /// </summary>
    private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(BO.TimeUnit.hour);
    }

    /// <summary>
    /// Click event handler for the Add One Day button.
    /// </summary>
    private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(BO.TimeUnit.day);
    }

    /// <summary>
    /// Click event handler for the Add One Month button.
    /// </summary>
    private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(BO.TimeUnit.month);
    }

    /// <summary>
    /// Click event handler for the Add One Year button.
    /// </summary>
    private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(BO.TimeUnit.year);
    }

    /// <summary>
    /// Click event handler for the Update Max Range button.
    /// </summary>
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

    /// <summary>
    /// Click event handler for the Volunteer List button.
    /// </summary>
    private void btnVolunteerList_Click(object sender, RoutedEventArgs e)
    {
        // Open the Volunteer List screen
        new Volunteer.VolunteerListWindow().Show();
    }

    /// <summary>
    /// Click event handler for the Reset Database button.
    /// </summary>
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

    /// <summary>
    /// Click event handler for the Initialize Database button.
    /// </summary>
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

    /// <summary>
    /// Performs a database operation and provides feedback to the user.
    /// </summary>
    /// <param name="dbOperation">The database operation to perform.</param>
    /// <param name="successMessage">The success message to display.</param>
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

    /// <summary>
    /// Closes all open windows except the main window.
    /// </summary>
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
