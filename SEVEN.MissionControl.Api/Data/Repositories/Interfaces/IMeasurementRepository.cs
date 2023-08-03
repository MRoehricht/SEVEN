using SEVEN.Core.Models;

namespace SEVEN.MissionControl.Api.Data.Repositories.Interfaces;

public interface IMeasurementRepository
{
    Task<IEnumerable<Measurement>> GetMeasurements();
    Task<Measurement?> GetLastMeasurement(Guid probeId);
    Task<Measurement?> CreateMeasurement(Measurement measurement);
}