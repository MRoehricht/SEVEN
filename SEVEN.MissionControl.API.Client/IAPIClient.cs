using SEVEN.Core.Models;

namespace SEVEN.Core.API.Client
{
    public interface IAPIClient
    {
        Task CreateRoverTask(RoverTask roverTask);
        Task<IEnumerable<RoverTask>> GetReadyRoverTasks(Guid roverId);
        Task<Rover?> GetRover(Guid roverId);
        Task<RoverTask?> GetRoverTask(Guid taskId);
        Task UpdateRoverTaskStatus(RoverTask roverTask);
    }
}