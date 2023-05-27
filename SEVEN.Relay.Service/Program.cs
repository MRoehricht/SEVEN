using SEVEN.MissionControl.API.Client.DependencyInjection;
using SEVEN.Rover.Core.DependencyInjection;

namespace SEVEN.Relay.Service;

public class Program
{
    public static void Main(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                IConfiguration configuration = hostContext.Configuration;
                services.AddRoverClient(configuration, hostContext.HostingEnvironment.IsDevelopment());
                services.AddAPIClient(configuration);
                services.AddHostedService<RelayService>();
            })
            .Build();

        host.Run();
    }
}