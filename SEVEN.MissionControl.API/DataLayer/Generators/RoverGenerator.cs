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

                var roverOne = new Rover
                {
                    Id = Guid.Parse("7A73F8AE-0000-0000-AAAA-7AB5A00A9C1D"),
                    Name = "Sandberg1",
                    GeoCoordinates = new GeoCoordinates("50.85953679509189", "11.19558185972183")
                };

                context.Rovers.Add(roverOne);

                context.RoverTasks.AddRange(
                new RoverTask
                {
                    Id = Guid.NewGuid(),
                    Position = 1,
                    RoverId = roverOne.Id,
                    Rover = roverOne,
                    Command = RoverTaskCommands.COMMAND_HEADLIGHTS_ON,
                    StatusUpdate = DateTime.Now.AddDays(-3),
                    Status = RoverTaskStatus.Ready,
                    StatusInfo = null
                }
                ,
                new RoverTask
                {
                    Id = Guid.NewGuid(),
                    Position = 2,
                    RoverId = roverOne.Id,
                    Rover = roverOne,
                    Command = RoverTaskCommands.COMMAND_CAMERA_TAKEFOTO,
                    StatusUpdate = DateTime.Now.AddDays(-2),
                    Status = RoverTaskStatus.Success,
                    StatusInfo = null
                },
                new RoverTask
                {
                    Id = Guid.NewGuid(),
                    Position = 3,
                    RoverId = roverOne.Id,
                    Rover = roverOne,
                    Command = RoverTaskCommands.COMMAND_HEADLIGHTS_OFF,
                    StatusUpdate = DateTime.Now.AddDays(-1),
                    Status = RoverTaskStatus.Success,
                    StatusInfo = null
                }
                );

                context.SaveChanges();
            }
        }
    }
}
