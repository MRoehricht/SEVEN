using System.ComponentModel.DataAnnotations;

namespace SEVEN.Core.Models;

public class Measurement
{
    public Guid Id { get; set; }
    [Required] public Guid ProbeId { get; set; }
    [Required] public MeasurementType MeasurementType { get; set; }
    [Required] public string Value { get; set; }
    public DateTime Time { get; set; }
}