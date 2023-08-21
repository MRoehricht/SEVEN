using Microsoft.EntityFrameworkCore;
using SEVEN.Core.Models;
using SEVEN.MissionControl.Api.Data.Contexts;
using SEVEN.MissionControl.Api.Data.Repositories.Interfaces;
using SEVEN.MissionControl.Api.Models.EventSystem;
using SEVEN.MissionControl.Api.Services.EventSystem;

namespace SEVEN.MissionControl.Api.Data.Repositories;

public class MeasurementRepository : IMeasurementRepository
{
    private readonly MissionControlContext _context;
    private readonly EventPublisher _eventPublisher;

    public MeasurementRepository(MissionControlContext context, EventPublisher eventPublisher)
    {
        _context = context;
        _eventPublisher = eventPublisher;
    }

    public async Task<IEnumerable<Measurement>> GetMeasurements()
    {
        return await _context.Measurements.Include(_ => _.Probe).AsNoTracking().ToListAsync();
    }

    public async Task<Measurement?> GetLastMeasurement(Guid probeId)
    {
        var measurements = await GetMeasurements();
        var measurement = measurements.Where(_ => _.ProbeId == probeId).MaxBy(_ => _.Time);
        return measurement;
    }


    public async Task<Measurement?> CreateMeasurement(Measurement measurement)
    {
        var probe = await _context.Probes.FindAsync(measurement.ProbeId);
        if (probe is null) return null;

        measurement.Id = Guid.NewGuid();
        measurement.Time = DateTime.UtcNow;

        await _context.Measurements.AddAsync(measurement);
        probe.LastContact = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        _eventPublisher.Raise(new ProbeChangedEventMessage{Value = measurement.Value, TargetId = probe.Id});
        return measurement;
    }

    public async Task<bool> DeleteMeasurement(Guid measurementId)
    {
        var measurement = await _context.Measurements.FindAsync(measurementId);
        if (measurement is null) return false;
        
        _context.Measurements.Remove(measurement);
        await _context.SaveChangesAsync();
        return true;
    }
}