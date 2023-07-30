using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SEVEN.Core.Models;

public class Measurement
{
    public Guid Id { get; set; }
    [Required] public Guid ProbeId { get; set; }
    [Required] public MeasurementType MeasurementType { get; set; }
    [Required] public string Value { get; set; }
    public DateTime Time { get; set; }
    public DateTime LocalTime => DateTime.SpecifyKind(Time, DateTimeKind.Utc).ToLocalTime();
    [JsonIgnore]
    public virtual Probe? Probe { get; set; }
}