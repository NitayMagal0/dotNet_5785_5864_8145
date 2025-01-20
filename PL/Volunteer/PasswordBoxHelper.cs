using System.Windows;
using System.Windows.Controls;

namespace PL.Volunteer
{
    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty BoundPasswordProperty =
            DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordBoxHelper),
                new FrameworkPropertyMetadata(string.Empty, OnBoundPasswordChanged));

        public static string GetBoundPassword(DependencyObject obj)
        {
            return (string)obj.GetValue(BoundPasswordProperty);
        }

        public static void SetBoundPassword(DependencyObject obj, string value)
        {
            obj.SetValue(BoundPasswordProperty, value);
        }

        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
                if (e.NewValue != null && passwordBox.Password != e.NewValue.ToString())
                {
                    passwordBox.Password = e.NewValue.ToString();
                }
                passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
            }
        }

        private static bool _isUpdating;

        private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_isUpdating) return;

            if (sender is PasswordBox passwordBox)
            {
                _isUpdating = true;
                SetBoundPassword(passwordBox, passwordBox.Password);
                _isUpdating = false;
            }
        }
    }
}