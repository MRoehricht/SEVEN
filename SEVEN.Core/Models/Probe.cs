using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SEVEN.Core.Models;

public class Probe
{
    [Key] public Guid Id { get; set; }

    [JsonIgnore]
    public string? ApiKey { get; set; }
    public string? Name { get; set; }
    public MeasurementType MeasurementsType { get; set; }
    public int SendingIntervalMinutes { get; set; }
    public ICollection<Measurement>? Measurements { get; set; }
    
    public DateTime? LastContact { get; set; }
}