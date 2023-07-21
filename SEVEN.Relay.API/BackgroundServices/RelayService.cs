using System.ComponentModel;
using SEVEN.Core.API.Client;
using SEVEN.Core.Models;
using SEVEN.Rover.Core.Clients;

namespace SEVEN.Relay.API.BackgroundServices;

public class RelayService : BackgroundService
{
    private readonly IAPIClient? _apiClient;
    private readonly ILogger<RelayService> _logger;
    private readonly IRoverClient? _roverClient;

    public RelayService(ILogger<RelayService> logger, IRoverClient roverClient, IAPIClient apiClient)
    {
        _logger = logger;
        _roverClient = roverClient;
        _apiClient = apiClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_roverClient == null)
        {
            _logger.LogError("RoverUrl ist nicht vergeben. Der Service wird beendet.");
            return;
        }

        if (_apiClient == null)
        {
            _logger.LogError("API Connection ist nicht vergeben. Der Service wird beendet.");
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);
            await WorkRoverTasks();
            await Task.Delay(10000, stoppingToken);
        }
    }


    private async Task WorkRoverTasks()
    {
        if (_apiClient is null || _roverClient is null) return;
        _logger.LogInformation("Lade bereite Tasks von der API: {Time}", DateTimeOffset.Now);
        var tasks = await _apiClient.GetReadyRoverTasks(_roverClient.RoverId);
        foreach (var task in tasks) await StartRoverTask(task);
    }


    private async Task StartRoverTask(RoverTask roverTask)
    {
        if (_apiClient is null || _roverClient is null) return;
        _logger.LogInformation($"Starte Task {roverTask.Id}: {{time}}", DateTimeOffset.Now);
        roverTask.Status = RoverTaskStatus.Started;
        await _apiClient.UpdateRoverTaskStatus(roverTask);

        switch (roverTask.Command)
        {
            case RoverTaskCommands.None:
                break;
            case RoverTaskCommands.CommandHeadlightsOn:
                await _roverClient.TurnHeadlights_On();
                break;
            case RoverTaskCommands.CommandHeadlightsOff:
                await _roverClient.TurnHeadlights_Off();
                break;
            case RoverTaskCommands.CommandCameraTakefoto:
                await _roverClient.TakeFoto();
                break;
            case RoverTaskCommands.CommandCameraMoveLeft:
                break;
            case RoverTaskCommands.CommandCameraMoveReight:
                break;
            default:
                roverTask.Status = RoverTaskStatus.Failed;
                roverTask.StatusInfo = $"Der Typ:{nameof(roverTask.Command)} ist Unbekannt!";
                await _apiClient.UpdateRoverTaskStatus(roverTask);
                _logger.LogError(roverTask.StatusInfo, DateTimeOffset.Now);
                throw new InvalidEnumArgumentException(nameof(roverTask.Command), (int)roverTask.Command,
                    roverTask.Command.GetType());
        }

        roverTask.Status = RoverTaskStatus.Success;
        await _apiClient.UpdateRoverTaskStatus(roverTask);
        _logger.LogInformation($"Task {roverTask.Id} erfolgreich beendet : {{time}}", DateTimeOffset.Now);
    }
}