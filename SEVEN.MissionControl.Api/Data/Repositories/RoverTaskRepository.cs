﻿using Microsoft.EntityFrameworkCore;
using SEVEN.Core.Models;
using SEVEN.MissionControl.Api.Data.Contexts;
using SEVEN.MissionControl.Api.Data.Repositories.Interfaces;

namespace SEVEN.MissionControl.Api.Data.Repositories;

public class RoverTaskRepository : IRoverTaskRepository
{
    private readonly MissionControlContext _context;

    public RoverTaskRepository(MissionControlContext context)
    {
        _context = context;
    }


    public async Task<RoverTask?> GetRoverTask(Guid roverTaskId)
    {
        return await _context.RoverTasks.FindAsync(roverTaskId);
    }

    public async Task<IEnumerable<RoverTask>> GetReadyRoverTasks(Guid roverId)
    {
        var tasks = await _context.RoverTasks.Where(t => t.RoverId == roverId && t.Status == RoverTaskStatus.Ready)
            .OrderBy(t => t.Position).ToListAsync();
        return tasks;
    }

    public async Task<RoverTask?> CreateRoverTask(Guid roverId, RoverTaskCommands command)
    {
        var roverTask = new RoverTask
        {
            Id = Guid.NewGuid(),
            Command = command,
            RoverId = roverId,
            Status = RoverTaskStatus.Ready
        };
        return await CreateRoverTask(roverTask);
    }

    public async Task<RoverTask?> CreateRoverTask(RoverTask roverTask)
    {
        var rover = await _context.Rovers.FindAsync(roverTask.RoverId);

        if (rover == null) return null;

        var positoin = await _context.RoverTasks.Where(t => t.RoverId == rover.Id).MaxAsync(t => t.Position);
        roverTask.StatusUpdate = DateTime.Now;
        roverTask.Position = ++positoin;
        _context.RoverTasks.Add(roverTask);
        await _context.SaveChangesAsync();
        return roverTask;
    }

    public async Task<RoverTask?> UpdateRoverTask(RoverTask inputRoverTask)
    {
        var roverTask = await _context.RoverTasks.FindAsync(inputRoverTask.Id);

        if (roverTask is null) return null;

        roverTask.StatusUpdate = DateTime.Now;
        roverTask.Status = inputRoverTask.Status;
        roverTask.StatusInfo = inputRoverTask.StatusInfo;

        await _context.SaveChangesAsync();

        return roverTask;
    }
}