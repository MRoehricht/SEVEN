namespace SEVEN.MissionControl.Api.Models.EventSystem;

public interface IEventMessage
{
    EventType EventType { get; }
    
    Guid TargetId { get; }
    
    string? Value { get; }
}