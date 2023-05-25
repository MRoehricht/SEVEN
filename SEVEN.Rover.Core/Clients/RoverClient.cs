using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SEVEN.Core.Models.Configuration;
using SEVEN.Rover.Core.Constants;
using SEVEN.Rover.Core.Models;

namespace SEVEN.Rover.Core.Clients
{
    public class RoverClient : IRoverClient
    {
        private readonly string _baseUri;
        public RoverStatus? RoverStatus { get; private set; }
        public RoverClient(IOptions<RoverConnection> options)
        {
            if (options.Value.RoverUrl == null)
            {
                throw new ArgumentException(nameof(options.Value.RoverUrl));
            }

            _baseUri = options.Value.RoverUrl;
        }

        public async Task TurnHeadlights_On()
        {
            await Call(RoverCommands.COMMAND_HEADLIGHTS_ON);
        }

        public async Task TurnHeadlights_Off()
        {
            await Call(RoverCommands.COMMAND_HEADLIGHTS_OFF);
        }

        public async Task<bool> GetHeadlights_Status()
        {
            await Call(RoverCommands.COMMAND_GET_STATUS);
            if (RoverStatus != null)
            {
                return RoverStatus.SwitchStatuses.FirstOrDefault(_ => _.Name == RoverStatusNames.STATUS_HEADLIGHTS)?.Status ?? false;
            }

            return false;
        }

        public async Task<string?> TakeFoto()
        {
            await Call(RoverCommands.COMMAND_CAMERA_TAKEFOTO);
            if (RoverStatus != null)
            {
                return RoverStatus.ImageData;
            }

            return null;
        }

        private async Task Call(string command)
        {
            using var handler = new HttpClientHandler();
            using var client = new HttpClient(handler);

            client.BaseAddress = new Uri(_baseUri);
            var response = await client.GetAsync(command);

            if (response != null)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                RoverStatus = JsonConvert.DeserializeObject<RoverStatus>(jsonString);
            }
        }
    }
}
