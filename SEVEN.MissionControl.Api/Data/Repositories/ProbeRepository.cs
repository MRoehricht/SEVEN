using System.Security.Cryptography;
using System.Text;
using SEVEN.Core.Models;
using SEVEN.MissionControl.Api.Data.Contexts;
using SEVEN.MissionControl.Api.Data.Repositories.Interfaces;

namespace SEVEN.MissionControl.Api.Data.Repositories;

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
        probe.ApiKey = GetApiKey(40);
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
            dbProbe.MeasurementsType = probe.MeasurementsType;

            await _context.SaveChangesAsync();
        }
        else
        {
            return null;
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
    
    private static string GetApiKey(int size)
    {
        var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
        var data = new byte[size];
        using (var crypto = RandomNumberGenerator.Create())
        {
            crypto.GetBytes(data);
        }
        var result = new StringBuilder(size);
        foreach (var b in data)
        {
            result.Append(chars[b % (chars.Length)]);
        }
        return result.ToString();
    }
}