using Microsoft.EntityFrameworkCore;
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




            app.Run();
        }
    }
}