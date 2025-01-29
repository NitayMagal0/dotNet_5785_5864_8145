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



    private const string ApiEndpoint = "https://api.openrouteservice.org/v2/directions/foot-walking";
    private const string _apiKey = "5b3ce3597851110001cf62484a4dcf303a1d4a5a936fe4d23963d50e";

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
            client.DefaultRequestHeaders.Add("User-Agent", "GeocodingApp/1.0");

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

    /// <summary>
    /// Takes an address and returns coordinates (latitude and longitude) asynchronously.
    /// </summary>
    /// <param name="address">The address to search for.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains coordinates (Latitude, Longitude).</returns>
    public static async Task<(double Latitude, double Longitude)> GetCoordinatesAsync(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            throw new ArgumentException("Address cannot be null or empty.", nameof(address));

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "GeocodingApp/1.0");
            string url = $"{BaseUrl}?q={Uri.EscapeDataString(address)}&format=xml";

            HttpResponseMessage response;
            try
            {
                response = await client.GetAsync(url).ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Error sending request to the API.", ex);
            }

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error: Unable to get data from API. Status code: {response.StatusCode}");

            string xml = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            XDocument xmlData = XDocument.Parse(xml);

            var firstResult = xmlData.Descendants("place").FirstOrDefault();
            if (firstResult == null)
                throw new Exception("No results found for the given address.");

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

    public static double GetResultAsync((double Latitude, double Longitude) start, (double Latitude, double Longitude) destination, DistanceType calculationType)
    {
        switch (calculationType)
        {
            case DistanceType.AirDistance:
                return CalculateAirDistance(start, destination);

            case DistanceType.WalkingDistance:
                return GetTravelTimeAsync(start, destination, "walking").Result;

            case DistanceType.DrivingDistance:
                return GetTravelTimeAsync(start, destination, "driving").Result;

            default:
                throw new ArgumentException("Invalid calculation type");
        }
    }

    private static readonly HttpClient httpClient = new HttpClient();

    /// <summary>
    /// Calculates the travel time in minutes between two geographical points using Google Distance Matrix API.
    /// </summary>
    /// <param name="start">Tuple containing Latitude and Longitude of the origin.</param>
    /// <param name="destination">Tuple containing Latitude and Longitude of the destination.</param>
    /// <param name="mode">Mode of travel (e.g., driving, walking).</param>
    /// <returns>Travel time in minutes.</returns>
    public static async Task<double> GetTravelTimeAsync(
        (double Latitude, double Longitude) start,
        (double Latitude, double Longitude) destination,
        string mode)
    {
        // Retrieve the API key from environment variables or secure configuration
        string apiKey = "AIzaSyBbYYTu1YEzHaBoHu7-e4fZd-OT8V6PScg";
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("Google Maps API key is not set in the environment variables.");
        }

        // Construct the origins and destinations parameters
        string origins = $"{start.Latitude},{start.Longitude}";
        string destinations = $"{destination.Latitude},{destination.Longitude}";

        // Properly encode URL parameters
        string url = $"https://maps.googleapis.com/maps/api/distancematrix/json" +
                     $"?origins={Uri.EscapeDataString(origins)}" +
                     $"&destinations={Uri.EscapeDataString(destinations)}" +
                     $"&mode={Uri.EscapeDataString(mode)}" +
                     $"&key={Uri.EscapeDataString(apiKey)}";

        try
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30))) // 30-second timeout
            {
                Console.WriteLine($"Sending request to URL: {url}");

                // Send the GET request with cancellation token and ConfigureAwait(false)
                HttpResponseMessage response = await httpClient.GetAsync(url, cts.Token).ConfigureAwait(false);
                Console.WriteLine("Received response from Google API.");

                // Ensure the HTTP response is successful
                response.EnsureSuccessStatusCode();

                // Read and parse the JSON response
                string jsonResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                using (JsonDocument document = JsonDocument.Parse(jsonResponse))
                {
                    JsonElement root = document.RootElement;

                    // Check the overall status of the API response
                    string status = root.GetProperty("status").GetString();
                    if (!string.Equals(status, "OK", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new Exception($"API returned status: {status}");
                    }

                    // Navigate through JSON to get duration in seconds
                    JsonElement rows = root.GetProperty("rows");
                    if (rows.GetArrayLength() == 0)
                    {
                        throw new Exception("No rows found in the API response.");
                    }

                    JsonElement elements = rows[0].GetProperty("elements");
                    if (elements.GetArrayLength() == 0)
                    {
                        throw new Exception("No elements found in the API response.");
                    }

                    JsonElement element = elements[0];
                    string elementStatus = element.GetProperty("status").GetString();
                    if (!string.Equals(elementStatus, "OK", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new Exception($"Element returned status: {elementStatus}");
                    }

                    JsonElement duration = element.GetProperty("duration");
                    if (!duration.TryGetProperty("value", out JsonElement durationValue))
                    {
                        throw new Exception("Duration value not found in the API response.");
                    }

                    int durationSeconds = durationValue.GetInt32();
                    double durationMinutes = durationSeconds / 60.0;

                    Console.WriteLine($"Travel time: {durationMinutes} minutes.");
                    return durationMinutes;
                }
            }
        }
        catch (TaskCanceledException)
        {
            throw new TimeoutException("The request to the Google Distance Matrix API timed out.");
        }
        catch (HttpRequestException httpEx)
        {
            throw new Exception($"HTTP error occurred: {httpEx.Message}", httpEx);
        }
        catch (JsonException jsonEx)
        {
            throw new Exception($"Error parsing JSON response: {jsonEx.Message}", jsonEx);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error calculating travel time: {ex.Message}", ex);
        }
    }
}



