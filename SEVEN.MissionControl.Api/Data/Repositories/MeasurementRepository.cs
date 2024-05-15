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
        return await _context.Measurements.Include(m => m.Probe).AsNoTracking().ToListAsync();
    }

    public async Task<Measurement?> GetLastMeasurement(Guid probeId, MeasurementType measurementType)
    {
        var measurements = await GetMeasurements();
        var measurement = measurements.Where(m => m.ProbeId == probeId && m.MeasurementType == measurementType).MaxBy(m => m.Time);
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

    public async Task<bool> DeleteMultiMeasurements(Guid probeId, MeasurementType? measurementType = null)
    {
        var probe = await _context.Probes.FindAsync(probeId);
        if (probe is null) return false;

       var measurements = measurementType.HasValue 
            ? _context.Measurements.Where(m => m.ProbeId == probeId && m.MeasurementType == measurementType.Value) 
            : _context.Measurements.Where(m => m.ProbeId == probeId);
        
        _context.Measurements.RemoveRange(measurements);
        await _context.SaveChangesAsync();
        return true;
    }
}