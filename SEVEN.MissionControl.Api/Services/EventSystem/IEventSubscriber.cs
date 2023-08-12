using SEVEN.MissionControl.Api.Models.EventSystem;

namespace SEVEN.MissionControl.Api.Services.EventSystem;

public interface IEventSubscriber
{
    EventType EventType { get; }

    void OnNotificationReceived(IEventMessage eventMessage);
}