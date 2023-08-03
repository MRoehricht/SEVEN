﻿using Microsoft.EntityFrameworkCore;
using SEVEN.Core.Models;
using SEVEN.MissionControl.Api.Data.Contexts;
using SEVEN.MissionControl.Api.Data.Repositories.Interfaces;

namespace SEVEN.MissionControl.Api.Data.Repositories;

public class MeasurementRepository : IMeasurementRepository
{
    private readonly MissionControlContext _context;

    public MeasurementRepository(MissionControlContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Measurement>> GetMeasurements()
    {
        return await _context.Measurements.Include(_ => _.Probe).AsNoTracking().ToListAsync();
    }

    public async Task<Measurement?> GetLastMeasurement(Guid probeId)
    {
        var measurements = await GetMeasurements();
        var measurement = measurements.Where(_ => _.ProbeId == probeId).OrderByDescending(_ => _.Time)?.FirstOrDefault();
        return measurement;
    }


    public async Task<Measurement?> CreateMeasurement(Measurement measurement)
    {
        var probe = await _context.Probes.FindAsync(measurement.ProbeId);
        if (probe is null) return null;

        measurement.Id = Guid.NewGuid();
        measurement.Time = DateTime.UtcNow;

        await _context.Measurements.AddAsync(measurement);
        await _context.SaveChangesAsync();
        return measurement;
    }
}