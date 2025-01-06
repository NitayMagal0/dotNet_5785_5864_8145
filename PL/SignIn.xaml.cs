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
using BO;

namespace PL
{
    /// <summary>
    /// Interaction logic for SignIn.xaml
    /// </summary>
    public partial class SignIn : Window
    {

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public SignIn()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

   
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string password = txtPassword.Password; // Retrieve the password as a string
                Role role = s_bl.Volunteer.SignIn(txtUserName.Text, password);
                
                if(role==BO.Role.Volunteer)
                {
                    int volunteerID = s_bl.Volunteer.GetIdByName(txtUserName.Text);
                    Volunteer.VolunteerWindow volunteerWindow = new Volunteer.VolunteerWindow(volunteerID);
                    volunteerWindow.Show();
                    //this.Close();
                }
                else if (role == BO.Role.Manager)
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    //this.Close();
                }
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately
                MessageBox.Show("Login failed: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
