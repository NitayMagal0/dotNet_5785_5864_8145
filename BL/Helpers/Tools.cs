using System.Text;

namespace Helpers;

internal static class Tools
{
    public static string ToStringProperty<T>(this T t)
    {
        if (t == null) return string.Empty;

        var type = t.GetType();
        var properties = type.GetProperties();
        var sb = new StringBuilder();

        foreach (var property in properties)
        {
            var value = property.GetValue(t, null);
            if (value is System.Collections.IEnumerable enumerable && !(value is string))
            {
                sb.Append($"{property.Name}=[");
                foreach (var item in enumerable)
                {
                    sb.Append($"{item.ToStringProperty()}, ");
                }

                if (sb.Length > 1) sb.Length -= 2; // Remove the last comma and space
                sb.Append("], ");
            }
            else
            {
                sb.Append($"{property.Name}={value?.ToStringProperty() ?? "null"}, ");
            }
        }

        if (sb.Length > 2) sb.Length -= 2; // Remove the last comma and space
        return sb.ToString();
    }
}

