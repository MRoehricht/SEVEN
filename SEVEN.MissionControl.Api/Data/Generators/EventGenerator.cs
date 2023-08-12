using SEVEN.MissionControl.Api.Services;

namespace SEVEN.MissionControl.Api.Data.Generators;

public static class EventGenerator
{
    public static void Initialize(IServiceCollection services)
    {
        services.BuildServiceProvider().GetRequiredService<ServerSendEventsService>();
    }
}