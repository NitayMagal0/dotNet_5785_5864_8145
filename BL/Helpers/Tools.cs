using System.Text;
using System;
using System.Linq;
using System.Net.Http;
using System.Xml.Linq;

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




    /// <summary>
    /// Takes an address and returns coordinates (latitude and longitude).
    /// </summary>
    /// <param name="address">The address to search for.</param>
    /// <returns>Coordinates (Latitude, Longitude).</returns>
    private const string BaseUrl = "https://nominatim.openstreetmap.org/search";
    public static (double Latitude, double Longitude) GetCoordinates(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new Exception("Address cannot be null or empty.");

            using (HttpClient client = new HttpClient())
            {
                // Add the required User-Agent header
                client.DefaultRequestHeaders.Add("User-Agent", "YourAppName/1.0");

                // Build the URL with the address as a parameter and format=xml
                string url = $"{BaseUrl}?q={Uri.EscapeDataString(address)}&format=xml";

                // Send a synchronous GET request
                HttpResponseMessage response = client.GetAsync(url).Result;
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Error: Unable to get data from API. Status code: {response.StatusCode}");

                // Read the response as XML
                string xml = response.Content.ReadAsStringAsync().Result;

                // Load the XML content
                XDocument xmlData = XDocument.Parse(xml);

                // Find the first result and extract latitude and longitude
                var firstResult = xmlData.Descendants("place").FirstOrDefault();
                if (firstResult == null)
                    throw new Exception("No results found for the given address.");

                // Extract coordinates from the XML elements
                double latitude = double.Parse(firstResult.Attribute("lat").Value);
                double longitude = double.Parse(firstResult.Attribute("lon").Value);

                return (latitude, longitude);
            }
        }
    }
    

