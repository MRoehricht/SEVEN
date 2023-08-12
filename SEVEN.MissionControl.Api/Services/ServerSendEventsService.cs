using SEVEN.MissionControl.Api.Models.EventSystem;
using SEVEN.MissionControl.Api.Services.EventSystem;

namespace SEVEN.MissionControl.Api.Services;

public class ServerSendEventsService : IEventSubscriber
{
    private readonly ILogger<ServerSendEventsService>  _logger;
    public EventType EventType => EventType.ProbeHasChange;
    
    public ServerSendEventsService(EventPublisher eventPublisher, ILogger<ServerSendEventsService> logger )
    {
        _logger = logger;
        eventPublisher.Subscribe(this);
    }

    public void OnNotificationReceived(IEventMessage eventMessage)
    {
        _logger.LogInformation($"EventMessage: {eventMessage.EventType}, {eventMessage.TargetId}, {eventMessage.Value}");
    }
}