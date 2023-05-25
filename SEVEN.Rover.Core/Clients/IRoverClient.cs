using SEVEN.Rover.Core.Models;

namespace SEVEN.Rover.Core.Clients
{
    public interface IRoverClient
    {
        RoverStatus? RoverStatus { get; }

        Task<bool> GetHeadlights_Status();
        Task<string?> TakeFoto();
        Task TurnHeadlights_Off();
        Task TurnHeadlights_On();
    }
}