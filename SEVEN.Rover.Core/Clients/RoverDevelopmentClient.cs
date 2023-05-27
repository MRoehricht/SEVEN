using Microsoft.Extensions.Logging;
using SEVEN.Rover.Core.Constants;
using SEVEN.Rover.Core.Models;

namespace SEVEN.Rover.Core.Clients
{
    public class RoverDevelopmentClient : IRoverClient
    {
        private bool _headlightsOn = false;
        private readonly ILogger<RoverDevelopmentClient> _logger;

        public Guid RoverId => Guid.Parse("7A73F8AE-0000-0000-AAAA-7AB5A00A9C1D");
        public RoverStatus? RoverStatus { get; private set; }

        public RoverDevelopmentClient(ILogger<RoverDevelopmentClient> logger)
        {
            RoverStatus = new RoverStatus { Id = "RoverDevelopmentClient" };
            _logger = logger;
        }

        public async Task<bool> GetHeadlights_Status()
        {
            await Task.Delay(100);
            return _headlightsOn;
        }

        public async Task<string?> TakeFoto()
        {
            await Task.Delay(1000);
            return "IMAGE DATA";
        }

        public Task TurnHeadlights_Off()
        {
            _headlightsOn = false;
            SetSwitchStatuses(RoverStatusNames.STATUS_HEADLIGHTS, false);
            return Task.CompletedTask;
        }

        public Task TurnHeadlights_On()
        {
            _headlightsOn = true;
            SetSwitchStatuses(RoverStatusNames.STATUS_HEADLIGHTS, true);
            return Task.CompletedTask;
        }

        private void SetSwitchStatuses(string name, bool status)
        {
            RoverStatus ??= new RoverStatus { Id = "RoverDevelopmentClient" };

            var item = RoverStatus.SwitchStatuses.FirstOrDefault(_ => _.Name == name);

            if (item == null)
            {
                RoverStatus.SwitchStatuses.Add(new SwitchStatus { Name = name, Status = status });
            }
            else
            {
                item.Status = status;
            }
            _logger.LogInformation($"{name}:{status}");
        }
    }
}
