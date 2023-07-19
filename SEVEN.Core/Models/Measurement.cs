namespace SEVEN.Core.Models;

public class Measurement
{
    public Guid Id { get; set; }
    public Guid ProbeId { get; set; }
    public MeasurementType MeasurementType { get; set; }
    public string? Value { get; set; }
    public DateTime Time { get; set; }
}