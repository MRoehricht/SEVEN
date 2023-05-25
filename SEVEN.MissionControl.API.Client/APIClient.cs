using Microsoft.Extensions.Options;
using SEVEN.Core.Models.Configuration;

namespace SEVEN.Core.API.Client
{
    public class APIClient
    {
        private readonly string _baseUrl;

        public APIClient(IOptions<APIConnection> options)
        {
            if (options?.Value?.BaseUrl == null)
            {
                throw new ArgumentNullException(nameof(APIConnection));
            }

            _baseUrl = options.Value.BaseUrl;
        }
    }
}
