using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SEVEN.Core.Models.Configuration;
using SEVEN.Rover.Core.Clients;

namespace SEVEN.Rover.Core.DependencyInjection;

public static class RoverClientServiceCollectionExtensions
{
    /// <summary>
    /// Pre-Condition:
    /// appsettings.json:
    /// "RoverConnection": { "RoverUrl": "..." }
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddRoverClient(
              this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RoverConnection>(configuration.GetSection(nameof(RoverConnection)));
        services.TryAddTransient<IRoverClient, RoverClient>();

        return services;
    }
}
