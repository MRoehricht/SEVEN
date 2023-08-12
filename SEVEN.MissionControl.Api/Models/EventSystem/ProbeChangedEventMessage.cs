namespace SEVEN.MissionControl.Api.Models.EventSystem;

public class ProbeChangedEventMessage: IEventMessage
{
    public EventType EventType => EventType.ProbeHasChange;
    public Guid TargetId { get; init; }
    public string? Value { get; init; }
}