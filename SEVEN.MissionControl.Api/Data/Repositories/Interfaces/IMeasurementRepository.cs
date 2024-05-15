using SEVEN.Core.Models;

namespace SEVEN.MissionControl.Api.Data.Repositories.Interfaces;

public interface IMeasurementRepository
{
    Task<IEnumerable<Measurement>> GetMeasurements();
    Task<Measurement?> GetLastMeasurement(Guid probeId, MeasurementType measurementType);
    Task<Measurement?> CreateMeasurement(Measurement measurement);
    Task<bool> DeleteMeasurement(Guid measurementId);
    Task<bool> DeleteMultiMeasurements(Guid probeId, MeasurementType? measurementType = null);
}