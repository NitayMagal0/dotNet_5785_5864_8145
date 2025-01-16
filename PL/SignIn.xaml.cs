using System;
using System.ComponentModel;  // Add this namespace for INotifyPropertyChanged
using System.Windows;
using System.Windows.Input;
using BO;

namespace PL
{
    public partial class SignIn : Window, INotifyPropertyChanged
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Identifies the ButtonText dependency property.
        /// </summary>
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(SignIn), new PropertyMetadata("Sign In"));

        /// <summary>
        /// Gets or sets the text for the button.
        /// </summary>
        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set
            {
                SetValue(ButtonTextProperty, value);
                OnPropertyChanged(nameof(ButtonText));  // Notify the UI about the change
            }
        }
        private string _userID;
        public string UserID
        {
            get => _userID;
            set
            {
                if (_userID != value)
                {
                    _userID = value;
                    OnPropertyChanged(nameof(UserID));
                }
            }
        }
        public SignIn()
        {
            InitializeComponent();
            
            DataContext = this;  // Set the DataContext to this instance of SignIn
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                string password = txtPassword.Password;
                int id;
                if (!int.TryParse(_userID, out id))
                {
                    throw new Exception("Invalid User ID");
                }

                Role role = s_bl.Volunteer.SignIn(id, password);

                if (role == BO.Role.Volunteer)
                {
                    Volunteer.VolunteerUpdateWindow volunteerMainWindow = new Volunteer.VolunteerUpdateWindow(id);
                    volunteerMainWindow.Show();
                }
                else if (role == BO.Role.Manager)
                {
                    Manager.ManagerChoice managerChoice = new Manager.ManagerChoice(id);
                    managerChoice.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Login failed: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
