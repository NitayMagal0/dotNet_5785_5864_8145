using System.Windows;
using System.Windows.Controls;

namespace PL.Volunteer;
/// <summary>
/// Provides attached properties and methods for binding a PasswordBox's password.
/// </summary>
public static class PasswordBoxHelper
{
    /// <summary>
    /// Identifies the BoundPassword attached property.
    /// </summary>
    public static readonly DependencyProperty BoundPasswordProperty =
        DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordBoxHelper),
            new FrameworkPropertyMetadata(string.Empty, OnBoundPasswordChanged));

    /// <summary>
    /// Gets the bound password from the specified DependencyObject.
    /// </summary>
    /// <param name="obj">The DependencyObject to get the password from.</param>
    /// <returns>The bound password.</returns>
    public static string GetBoundPassword(DependencyObject obj)
    {
        return (string)obj.GetValue(BoundPasswordProperty);
    }


    /// <summary>
    /// Sets the bound password on the specified DependencyObject.
    /// </summary>
    /// <param name="obj">The DependencyObject to set the password on.</param>
    /// <param name="value">The password to set.</param>
    public static void SetBoundPassword(DependencyObject obj, string value)
    {
        obj.SetValue(BoundPasswordProperty, value);
    }


    /// <summary>
    /// Handles changes to the BoundPassword attached property.
    /// </summary>
    /// <param name="d">The DependencyObject on which the property value changed.</param>
    /// <param name="e">Event data for the property change.</param>
    private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is PasswordBox passwordBox)
        {
            passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
            if (e.NewValue != null)
            {
                passwordBox.Password = e.NewValue.ToString();
            }
            passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
        }
    }
    /// <summary>
    /// Handles the PasswordChanged event of the PasswordBox.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event data for the event.</param>
    private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            SetBoundPassword(passwordBox, passwordBox.Password);
        }
    }
}


