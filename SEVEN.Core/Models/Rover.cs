using System.ComponentModel.DataAnnotations;

namespace SEVEN.Core.Models;

public class Rover
{
    [Key] public Guid Id { get; set; }

    public string? Name { get; set; }
    public ICollection<RoverTask> Tasks { get; set; } = new List<RoverTask>();
    public GeoCoordinates GeoCoordinates { get; set; }
}