using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Serialization;
using BO;

namespace PL.Manager
{
    /// <summary>
    /// Interaction logic for ManageCalls.xaml
    /// </summary>
    public partial class ManageCalls : Window
    {
        public ObservableCollection<CallInList> CallList { get; set; }
        public CallInList SelectedCall { get; set; }

        public ManageCalls()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Logic for adding a new call
            MessageBox.Show("Add Call button clicked.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteCallButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCall != null)
            {
                if (SelectedCall.Status == "Open" && SelectedCall.AssignmentsCount == 0)
                {
                    CallList.Remove(SelectedCall);
                    MessageBox.Show("Call deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Only open calls with no assignments can be deleted.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("No call selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CallList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCall != null)
            {
                // Open a new window for managing the selected call
                MessageBox.Show($"Managing call with ID: {SelectedCall.Id}", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Logic for filtering based on ComboBox selection
            MessageBox.Show("Filter selection changed.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public class CallInList
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string RemainingTime { get; set; }
        public string LastVolunteer { get; set; }
        public int AssignmentsCount { get; set; }
    }
}


