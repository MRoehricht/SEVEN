using SEVEN.Core.Models;

namespace SEVEN.MissionControl.Server.Services.Interfaces;

public interface IUserService
{
    Task<SevenUser?> GetCurrentUser();
}