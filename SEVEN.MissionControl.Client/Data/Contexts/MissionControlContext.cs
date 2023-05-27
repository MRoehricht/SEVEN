using Microsoft.EntityFrameworkCore;
using SEVEN.Core.Models;

namespace SEVEN.MissionControl.API.DataLayer.Context
{
    public class MissionControlContext : DbContext
    {
        public MissionControlContext(DbContextOptions<MissionControlContext> options)
            : base(options) { }

        public DbSet<Rover> Rovers { get; set; }
        public DbSet<RoverTask> RoverTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rover>()
            .HasMany(e => e.Tasks)
            .WithOne(e => e.Rover)
            .HasForeignKey(e => e.RoverId)
            .IsRequired();
        }
    }
}
