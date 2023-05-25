using SEVEN.Rover.Core.Clients;

namespace SEVEN.Relay.Service
{
    public class RelayService : BackgroundService
    {
        private readonly ILogger<RelayService> _logger;
        private readonly IRoverClient _roverClient;

        public RelayService(ILogger<RelayService> logger, IRoverClient roverClient)
        {
            _logger = logger;
            _roverClient = roverClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_roverClient == null)
            {
                _logger.LogError("RoverUrl ist nicht vergeben. Der Service wird beendet.");
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                _logger.LogInformation($"Url:{_roverClient}");
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}