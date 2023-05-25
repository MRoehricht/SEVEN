using Microsoft.EntityFrameworkCore;
using SEVEN.Core.Models;
using SEVEN.MissionControl.API.DataLayer.Context;

namespace SEVEN.MissionControl.API.DataLayer.Generators
{
    public class RoverGenerator
    {
        public static void Initialize(IServiceCollection services)
        {
            using (var context = new MissionControlContext(services.BuildServiceProvider().GetRequiredService<DbContextOptions<MissionControlContext>>()))
            {
                // Look for any board games.
                if (context.Rovers.Any())
                {
                    return;   // Data was already seeded
                }

                context.Rovers.Add(
                    new Rover
                    {
                        Id = Guid.Parse("7A73F8AE-0000-0000-AAAA-7AB5A00A9C1D"),
                        Name = "Sandberg1"
                    });




                context.SaveChanges();
            }
        }
    }
}
