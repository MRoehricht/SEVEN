using Microsoft.EntityFrameworkCore;
using SEVEN.Core.Models;
using SEVEN.MissionControl.Server.Data.Contexts;

namespace SEVEN.MissionControl.Server.API.Endpoints
{
    public static class RoverTaskEndpoint
    {
        public static RouteGroupBuilder RoverTaskGroup(this RouteGroupBuilder group)
        {

            group.MapGet("/{id}", GetRoverTask).WithName("GetRovertask").WithOpenApi();
            group.MapGet("/ready/{id}", GetReadyRoverTask).WithName("GetRovertaskToStart").WithOpenApi();
            group.MapPost("/", CreateRoverTask).WithName("CreateRovertask").WithOpenApi();
            group.MapPut("/", UpdateRoverTask).WithName("UpdateRoverTask").WithOpenApi();
            return group;
        }


        private static async Task<IResult> GetRoverTask(Guid id, HttpContext httpContext, MissionControlContext context)
        {
            return await context.RoverTasks.FindAsync(id) is RoverTask roverTask ? Results.Ok(roverTask) : Results.NotFound();
        }

        private static IResult GetReadyRoverTask(Guid id, HttpContext httpContext, MissionControlContext context)
        {
            var tasks = context.RoverTasks.Where(_ => _.RoverId == id && _.Status == RoverTaskStatus.Ready).OrderBy(_ => _.Position);
            return Results.Ok(tasks);
        }

        private static async Task<IResult> CreateRoverTask(RoverTask roverTask, HttpContext httpContext, MissionControlContext context)
        {
            var rover = await context.Rovers.FindAsync(roverTask.RoverId);

            if (rover == null) return Results.NotFound();

            var positoin = await context.RoverTasks.Where(_ => _.RoverId == rover.Id).MaxAsync(_ => _.Position);
            roverTask.StatusUpdate = DateTime.Now;
            roverTask.Position = ++positoin;
            context.RoverTasks.Add(roverTask);
            await context.SaveChangesAsync();

            return Results.Created($"/tasks/{roverTask.Id}", roverTask);
        }

        private static async Task<IResult> UpdateRoverTask(RoverTask inputRoverTask, HttpContext httpContext, MissionControlContext context)
        {
            var roverTask = await context.RoverTasks.FindAsync(inputRoverTask.Id);

            if (roverTask is null) return Results.NotFound();

            roverTask.StatusUpdate = DateTime.Now;
            roverTask.Status = inputRoverTask.Status;
            roverTask.StatusInfo = inputRoverTask?.StatusInfo;

            await context.SaveChangesAsync();

            return Results.NoContent();
        }
    }
}
