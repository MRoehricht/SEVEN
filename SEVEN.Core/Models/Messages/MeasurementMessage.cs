namespace SEVEN.Core.Models.Messages;

public class MeasurementMessage
{
    public Guid ProbeId { get; set; }

    public List<MeasurementItem> MeasurementItems { get; set; } = new();
}

