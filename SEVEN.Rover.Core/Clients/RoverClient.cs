using Newtonsoft.Json;
using SEVEN.Rover.Core.Constants;
using SEVEN.Rover.Core.Models;

namespace SEVEN.Rover.Core.Clients
{
    public class RoverClient
    {
        private readonly string _baseUri;

        public RoverStatus? RoverStatus { get; private set; }

        public RoverClient(string baseUri)
        {
            _baseUri = baseUri;
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
            var roverStatus = await GetRoverStatus();
            if (roverStatus != null)
            {
                return roverStatus.SwitchStatuses.FirstOrDefault(_ => _.Name == RoverStatusNames.STATUS_HEADLIGHTS)?.Status ?? false;
            }

            return false;
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

        private async Task<RoverStatus?> GetRoverStatus()
        {
            using var handler = new HttpClientHandler();
            using var client = new HttpClient(handler);

            client.BaseAddress = new Uri(_baseUri);
            var response = await client.GetAsync("");

            if (response != null)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<RoverStatus>(jsonString);
            }

            return null;
        }
    }
}
