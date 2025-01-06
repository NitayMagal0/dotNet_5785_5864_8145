using System;
using System.Collections.Generic;
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

namespace PL.Manager
{
    /// <summary>
    /// Interaction logic for ManagerChoice.xaml
    /// </summary>
    public partial class ManagerChoice : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public string WelcomeMessage { get; set; }
        public string ManagerName { get; set; }
        public int ManagerId { get; set; }

        public ManagerChoice(int id)
        {
            InitializeComponent();
            ManagerName = s_bl.Volunteer.GetNameById(id);
            WelcomeMessage = $"Welcome, {ManagerName}";
            DataContext = this;
            ManagerId = id;
        }

        private void btnMainManagementWindow_Click(object sender, RoutedEventArgs e)
        {
            // Open the Manage Volunteers window
             MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
           // this.Close();
        }

        private void btnVolunteerWindow_Click(object sender, RoutedEventArgs e)
        {
            // Open the Volunteer window
            Volunteer.VolunteerWindow volunteerWindow = new Volunteer.VolunteerWindow(ManagerId);
            volunteerWindow.Show();
            //this.Close();
        }
    }
}
