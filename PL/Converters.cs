using System;
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
        return !(value == null || value.ToString() == "0");
    }

    /// <summary>
    /// ConvertBack is not implemented.
    /// </summary>
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
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts a boolean value to Visibility.Collapsed for false or Visibility.Visible for true.
/// </summary>
public class BooleanToVisibilityInverseConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isActive)
        {
            return isActive ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts an integer value to a boolean based on whether it equals zero.
/// </summary>
public class IntToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int intValue)
        {
            // If parameter is "False", invert the logic
            bool invert = parameter != null && bool.TryParse(parameter.ToString(), out var result) && result;
            return invert ? intValue == 0 : intValue > 0;
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
