using System.Globalization;
using System.Windows.Data;

namespace PL;
public class IdToReadOnlyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // If the ID is null or the default value (e.g., "0"), it's in add mode (editable).
        // Otherwise, it's in update mode (read-only).
        return !(value == null || value.ToString() == "0");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}


public class DistanceTypeToLabelConverter : IValueConverter
{
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

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class NullToFalseConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value != null; // Enables the TextBox only if DistanceType is selected
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}