namespace SEVEN.Core.Models; 

public class MeasurementFilter {
    public Guid? ProbeId { get; set; }
    public DateOnly? Date { get; set; }
    public MeasurementType? Type { get; set; }
}
