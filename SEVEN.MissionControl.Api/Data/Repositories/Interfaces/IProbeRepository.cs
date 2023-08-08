using SEVEN.Core.Models;

namespace SEVEN.MissionControl.Api.Data.Repositories.Interfaces;

public interface IProbeRepository
{
    Task<Probe> CreateProbe(Probe probe);
    Task<Probe?> UpdateProbe(Probe probe);
    Task<bool> RemoveProbe(Guid id);
}