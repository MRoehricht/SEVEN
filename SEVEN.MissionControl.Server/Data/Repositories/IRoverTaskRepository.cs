using SEVEN.Core.Models;

namespace SEVEN.MissionControl.Server.Data.Repositories
{
    public interface IRoverTaskRepository
    {
        Task<RoverTask?> CreateRoverTask(RoverTask roverTask);
        Task<RoverTask?> CreateRoverTask(Guid roverId, RoverTaskCommands command);
        Task<IEnumerable<RoverTask>> GetReadyRoverTasks(Guid roverId);
        Task<RoverTask?> GetRoverTask(Guid roverTaskId);
        Task<RoverTask?> UpdateRoverTask(RoverTask inputRoverTask);
    }
}