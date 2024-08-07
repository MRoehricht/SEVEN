﻿using SEVEN.Core.Models;
using SEVEN.MissionControl.Api.Data.Contexts;

namespace SEVEN.MissionControl.Api.Data.Generators;

public class RoverGenerator
{
    public static void Initialize(MissionControlContext context)
    {
        // Look for any board games.
        if (context.Rovers.Any()) return; // Data was already seeded

        var roverOne = new Rover
        {
            Id = Guid.Parse("7A73F8AE-0000-0000-AAAA-7AB5A00A9C1D"),
            Name = "Sandberg1",
            //GeoCoordinates = new GeoCoordinates("50.85953679509189", "11.19558185972183")
        };

        var probeOne = new Probe
        {
            Id = Guid.Parse("7A73F8AE-0000-0000-BBBB-7AB5A00A9C1D"),
            ApiKey = "asotoiwes123afq234€werwik230g",
            LastContact = null,
            Name = "Probe1",
            MeasurementsType = MeasurementType.Temperature | MeasurementType.Humidity,
            SendingIntervalMinutes = 5
        };

        context.Probes.Add(probeOne);
        context.Rovers.Add(roverOne);

        context.RoverTasks.AddRange(
            new RoverTask
            {
                Id = Guid.NewGuid(),
                Position = 1,
                RoverId = roverOne.Id,
                Rover = roverOne,
                Command = RoverTaskCommands.CommandHeadlightsOn,
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
                Command = RoverTaskCommands.CommandCameraTakefoto,
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
                Command = RoverTaskCommands.CommandHeadlightsOff,
                StatusUpdate = DateTime.Now.AddDays(-1),
                Status = RoverTaskStatus.Success,
                StatusInfo = null
            }
        );

        context.SaveChanges();
    }
}