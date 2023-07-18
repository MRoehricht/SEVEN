using SEVEN.Core.Models;
using SEVEN.MissionControl.Server.Data.Contexts;

namespace SEVEN.MissionControl.Server.Data.Repositories;

public class ProbeRepository : IProbeRepository
{
    private readonly MissionControlContext _context;
    public ProbeRepository(MissionControlContext context)
    {
        _context = context;
    }
    
    public async Task<Probe> CreateProbe(Probe probe)
    {
        probe.Id = Guid.NewGuid();
        await _context.AddAsync(probe);
        await _context.SaveChangesAsync();
        return probe;
    }

    public async Task<Probe?> UpdateProbe(Probe probe)
    {
        var dbProbe = await _context.Probes.FindAsync(probe.Id);
        if (dbProbe != null)
        {
            dbProbe.Name = probe.Name;
            dbProbe.Measurements = probe.Measurements;

            await _context.SaveChangesAsync();
        }
        else
        {
            probe = null;
        }

        return probe;
    }

    public async Task<bool> RemoveProbe(Guid id)
    {
        var dbProbe = await _context.Probes.FindAsync(id);
        if (dbProbe != null)
        {
            _context.Probes.Remove(dbProbe);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }
}