using Microsoft.EntityFrameworkCore;
using SEVEN.Core.Models;
using SEVEN.MissionControl.API.DataLayer.Context;
using SEVEN.MissionControl.API.DataLayer.Generators;

namespace SEVEN.Core.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            builder.Services.AddDbContext<MissionControlContext>(options => options.UseInMemoryDatabase(databaseName: "MissionControlContextDB"));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            RoverGenerator.Initialize(builder.Services);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapGet("/rover/{id}", async (HttpContext httpContext, MissionControlContext context, Guid id) =>
            {
                var rover = await context.Rovers.FindAsync(id);
                if (rover != null)
                {
                    context.Entry(rover).Collection(b => b.Tasks).Load();
                    return Results.Ok(rover);
                }

                return Results.NotFound();

            })
            .WithName("GetRover")
            .WithOpenApi();


            app.MapGet("/tasks/{id}", async (Guid id, MissionControlContext context) => await context.RoverTasks.FindAsync(id) is RoverTask roverTask ? Results.Ok(roverTask) : Results.NotFound());

            app.MapGet("/tasks/ready/{id}", (HttpContext httpContext, MissionControlContext context, Guid id) =>
            {
                var tasks = context.RoverTasks.Where(_ => _.RoverId == id && _.Status == Models.RoverTaskStatus.Ready).OrderBy(_ => _.Position);
                return Results.Ok(tasks);
            })
            .WithName("GetRovertaskToStart")
            .WithOpenApi();

            app.MapPost("/tasks", async (RoverTask roverTask, HttpContext httpContext, MissionControlContext context) =>
            {
                var rover = await context.Rovers.FindAsync(roverTask.RoverId);

                if (rover == null) return Results.NotFound();

                var positoin = await context.RoverTasks.Where(_ => _.RoverId == rover.Id).MaxAsync(_ => _.Position);
                roverTask.StatusUpdate = DateTime.Now;
                roverTask.Position = ++positoin;
                context.RoverTasks.Add(roverTask);
                await context.SaveChangesAsync();

                return Results.Created($"/tasks/{roverTask.Id}", roverTask);
            })
            .WithName("CreateRovertask")
            .WithOpenApi();

            app.MapPut("/tasks", async (RoverTask inputRoverTask, HttpContext httpContext, MissionControlContext context) =>
            {
                var roverTask = await context.RoverTasks.FindAsync(inputRoverTask.Id);

                if (roverTask is null) return Results.NotFound();

                roverTask.StatusUpdate = DateTime.Now;
                roverTask.Status = inputRoverTask.Status;
                roverTask.StatusInfo = inputRoverTask?.StatusInfo;

                await context.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithName("UpdateRovertaskStatus")
            .WithOpenApi();




            app.Run();
        }
    }
}