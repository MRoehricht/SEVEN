using SEVEN.Core.Models;
using SEVEN.MissionControl.Server.Data.Repositories.Interfaces;

namespace SEVEN.MissionControl.Server.API.Endpoints;

public static class RoverTaskEndpoint
{
    public static RouteGroupBuilder RoverTaskGroup(this RouteGroupBuilder group)
    {
        group.MapGet("/{id}", GetRoverTask).WithName("GetRovertask").WithOpenApi();
        group.MapGet("/ready/{id}", GetReadyRoverTasks).WithName("GetRovertaskToStart").WithOpenApi();
        group.MapPost("/", CreateRoverTask).WithName("CreateRovertask").WithOpenApi();
        group.MapPut("/", UpdateRoverTask).WithName("UpdateRoverTask").WithOpenApi();
        return group;
    }


    private static async Task<IResult> GetRoverTask(Guid id, HttpContext httpContext, IRoverTaskRepository repository)
    {
        var roverTask = await repository.GetRoverTask(id);
        return roverTask is null ? Results.NotFound() : Results.Ok(roverTask);
    }

    private static async Task<IResult> GetReadyRoverTasks(Guid id, HttpContext httpContext,
        IRoverTaskRepository repository)
    {
        var tasks = await repository.GetReadyRoverTasks(id);
        return Results.Ok(tasks);
    }

    private static async Task<IResult> CreateRoverTask(RoverTask roverTask, HttpContext httpContext,
        IRoverTaskRepository repository)
    {
        var createdRoverTask = await repository.CreateRoverTask(roverTask);
        return createdRoverTask is null
            ? Results.NotFound()
            : Results.Created($"/tasks/{createdRoverTask.Id}", createdRoverTask);
    }

    private static async Task<IResult> UpdateRoverTask(RoverTask inputRoverTask, HttpContext httpContext,
        IRoverTaskRepository repository)
    {
        var updatedRoverTask = await repository.UpdateRoverTask(inputRoverTask);
        return updatedRoverTask is null ? Results.NotFound() : Results.NoContent();
    }
}