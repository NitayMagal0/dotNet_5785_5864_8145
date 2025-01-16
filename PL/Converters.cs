using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PL;
/// <summary>
/// Converts an ID to a read-only state. If the ID is null or "0", it is editable (add mode).
/// Otherwise, it is read-only (update mode).
/// </summary>
public class IdToReadOnlyConverter : IValueConverter
{
    /// <summary>
    /// Converts an ID to a read-only state.
    /// </summary>
    /// <param name="value">The ID value.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>True if the ID is not null and not "0", otherwise false.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // If the ID is null or the default value (e.g., "0"), it's in add mode (editable).
        // Otherwise, it's in update mode (read-only).
        return !(value == null || value.ToString() == "0");
    }

    /// <summary>
    /// ConvertBack is not implemented.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>Throws NotImplementedException.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}


/// <summary>
/// Converts a DistanceType to a label string.
/// </summary>
public class DistanceTypeToLabelConverter : IValueConverter
{
    /// <summary>
    /// Converts a DistanceType to a label string.
    /// </summary>
    /// <param name="value">The DistanceType value.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>A label string based on the DistanceType.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return "Max Distance For Call:";

        switch (value.ToString())
        {
            case "AirDistance":
                return "Max Distance For Call (km):";
            case "DrivingDistance":
            case "WalkingDistance":
                return "Max Distance For Call (minutes):";
            default:
                return "Max Distance For Call:";
        }
    }

    /// <summary>
    /// ConvertBack is not implemented.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>Throws NotImplementedException.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts a null value to false. Enables the TextBox only if DistanceType is selected.
/// </summary>
public class NullToFalseConverter : IValueConverter
{
    /// <summary>
    /// Converts a null value to false.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>True if the value is not null, otherwise false.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value != null; // Enables the TextBox only if DistanceType is selected
    }

    /// <summary>
    /// ConvertBack is not implemented.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>Throws NotImplementedException.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}


public class BooleanToVisibilityInverseConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isActive)
        {
            return isActive ?  Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}