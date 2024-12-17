using System.Text;
using System;
using System.Linq;
using System.Net.Http;
using System.Xml.Linq;
using System.Text.Json;
using BO;

namespace Helpers;

internal static class Tools
{
    private const string ApiEndpoint = "https://api.openrouteservice.org/v2/directions/foot-walking";
    private const string _apiKey = "5b3ce3597851110001cf62484a4dcf303a1d4a5a936fe4d23963d50e";
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


    public static double CalculateAirDistance((double Latitude, double Longitude)? start, (double Latitude, double Longitude)? destination)
    {
        if (start == null || destination == null)
        {
            throw new ArgumentNullException("Start and destination coordinates cannot be null.");
        }

        const double EarthRadiusKm = 6371;

        double toRadians(double angle) => Math.PI * angle / 180.0;

        var lat1 = toRadians(start.Value.Latitude);
        var lon1 = toRadians(start.Value.Longitude);
        var lat2 = toRadians(destination.Value.Latitude);
        var lon2 = toRadians(destination.Value.Longitude);

        var dLat = lat2 - lat1;
        var dLon = lon2 - lon1;

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c; // Returns distance in kilometers
    }

    internal static async Task<double> GetTravelTimeAsync((double Latitude, double Longitude) start, (double Latitude, double Longitude) destination, string profile)
    {
        using var httpClient = new HttpClient();

        var requestUri = $"{ApiEndpoint}{profile}?api_key={_apiKey}&start={start.Longitude},{start.Latitude}&end={destination.Longitude},{destination.Latitude}";

        try
        {
            var response = await httpClient.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(jsonResponse);

            var durationInSeconds = jsonDoc.RootElement
                .GetProperty("features")[0]
                .GetProperty("properties")
                .GetProperty("segments")[0]
                .GetProperty("duration").GetDouble();

            return Math.Round(durationInSeconds / 60, 2); // Returns duration in minutes
        }
        catch (Exception)
        {
            return -1; // Return -1 on error
        }
    }

    public static double GetResultAsync((double Latitude, double Longitude) start, (double Latitude, double Longitude) destination, DistanceType calculationType)
    {
        switch (calculationType)
        {
            case DistanceType.AirDistance:
                return CalculateAirDistance(start, destination);

            case DistanceType.WalkingDistance:
                return  GetTravelTimeAsync(start, destination, "foot-walking").Result;

            case DistanceType.DrivingDistance:
                return GetTravelTimeAsync(start, destination, "driving-car").Result;

            default:
                throw new ArgumentException("Invalid calculation type");
        }
    }

}



