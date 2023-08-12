using SEVEN.MissionControl.Api.Models.EventSystem;

namespace SEVEN.MissionControl.Api.Services.EventSystem;

public class EventPublisher : IEventPublisher
{
    private readonly List<IEventSubscriber> _eventSubscribers = new();
    
    public void Subscribe(IEventSubscriber eventSubscriber)
    {
        if (!_eventSubscribers.Contains(eventSubscriber))
        {
            _eventSubscribers.Add(eventSubscriber);
        }
    }

    public void Unsubscribe(IEventSubscriber eventSubscriber)
    {
        if (_eventSubscribers.Contains(eventSubscriber))
        {
            _eventSubscribers.Remove(eventSubscriber);
        }
    }

    public void Raise(IEventMessage eventMessage)
    {
        _eventSubscribers.Where(s => s.EventType == eventMessage.EventType).ToList().ForEach(s => s.OnNotificationReceived(eventMessage));
    }
}