namespace SEVEN.Core.Models;

public class MeasurementFilter
{
    public Guid? ProbeId { get; set; }
    public int? Year { get; set; }
    public int? Month { get; set; }
    public int? Day { get; set; }
    public MeasurementType? Type { get; set; }
    public bool? ReduceData { get; set; }
}
