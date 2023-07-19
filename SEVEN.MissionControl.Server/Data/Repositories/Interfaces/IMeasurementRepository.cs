using SEVEN.Core.Models;

namespace SEVEN.MissionControl.Server.Data.Repositories.Interfaces;

public interface IMeasurementRepository
{
    Task<IEnumerable<Measurement>> GetMeasurements();
    Task<Measurement?> CreateMeasurement(Measurement measurement);
}