using SEVEN.MissionControl.Api.Models.EventSystem;

namespace SEVEN.MissionControl.Api.Services.EventSystem;

public interface IEventPublisher
{
    void Subscribe(IEventSubscriber eventSubscriber);
    
    void Unsubscribe(IEventSubscriber eventSubscriber);

    void Raise(IEventMessage eventMessage);
}