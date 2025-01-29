using System.ComponentModel;

namespace BO;

public class VolunteerInList 
{
    public int Id { get; init; }
    public string? FullName { get; set; }
    public bool IsActive { get; set; }
    public int HandledCalls { get; set; }
    public int CanceledCalls { get; set; }
    public int ExpiredCalls { get; set; }
    public int? CurrentCallId { get; set; }
    public CallType CallType { get; set; }

    //------------------------------------------------ I added this so we could change the datagrid in VolunteerListWindow to show if the coordinates are missing
    // Add Nullable Latitude and Longitude properties
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    // Property to check if coordinates are missing
    public bool IsMissingCoordinates => Latitude == null || Longitude == null;
    //------------------------------------------------


    public override string ToString()
    {
        return $"Id: {Id}\n" +
               $"FullName: {FullName}\n" +
               $"IsActive: {IsActive}\n" +
               $"HandledCalls: {HandledCalls}\n" +
               $"CanceledCalls: {CanceledCalls}\n" +
               $"ExpiredCalls: {ExpiredCalls}\n" +
               $"CallsInProgress: {CurrentCallId}\n" +
               $"CallType: {CallType}";
    }
}


