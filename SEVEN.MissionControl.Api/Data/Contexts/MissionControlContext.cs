using Microsoft.EntityFrameworkCore;
using SEVEN.Core.Models;

namespace SEVEN.MissionControl.Api.Data.Contexts;

public class MissionControlContext : DbContext
{
    public MissionControlContext(DbContextOptions<MissionControlContext> options)
        : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    }

    public DbSet<Rover> Rovers { get; set; }
    public DbSet<RoverTask> RoverTasks { get; set; }
    public DbSet<Probe> Probes { get; set; }
    public DbSet<Measurement> Measurements { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rover>()
            .HasMany(e => e.Tasks)
            .WithOne(e => e.Rover)
            .HasForeignKey(e => e.RoverId)
            .IsRequired();

        modelBuilder.Entity<Probe>()
            .HasMany(p => p.Measurements)
            .WithOne(m => m.Probe)
            .HasForeignKey(m => m.ProbeId);
    }
}