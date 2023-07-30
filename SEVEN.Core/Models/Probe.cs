using System.ComponentModel.DataAnnotations;

namespace SEVEN.Core.Models;

public class Probe
{
    [Key] public Guid Id { get; set; }

    public string? Name { get; set; }
    public MeasurementType MeasurementsType { get; set; }
    public int SendingIntervalMinutes { get; set; }
    public ICollection<Measurement>? Measurements { get; set; }
}