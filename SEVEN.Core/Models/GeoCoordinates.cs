namespace SEVEN.Core.Models;

public struct GeoCoordinates
{
    public GeoCoordinates(string latitude, string longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
}
