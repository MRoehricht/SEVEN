using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SEVEN.Core.API.Client;
using SEVEN.Core.Models.Configuration;

namespace SEVEN.MissionControl.API.Client.DependencyInjection
{
    public static class APIClientServiceCollectionExtensions
    {
        /// <summary>
        /// Pre-Condition:
        /// appsettings.json:
        /// "APIConnection": { "BaseUrl": "https://..." }
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddAPIClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<APIConnection>(configuration.GetSection(nameof(APIConnection)));
            services.TryAddTransient<IAPIClient, APIClient>();

            return services;
        }
    }
}
