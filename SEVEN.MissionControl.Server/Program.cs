using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using SEVEN.MissionControl.Server.API.Endpoints;
using SEVEN.MissionControl.Server.Data;
using SEVEN.MissionControl.Server.Data.Contexts;
using SEVEN.MissionControl.Server.Data.Generators;
using SEVEN.MissionControl.Server.Data.Repositories;

namespace SEVEN.MissionControl.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<MissionControlContext>(options => options.UseInMemoryDatabase(databaseName: "MissionControlContextDB"));

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddMudServices();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddSingleton<WeatherForecastService>();
            builder.Services.AddTransient<IRoverTaskRepository, RoverTaskRepository>();
            builder.Services.AddTransient<IProbeRepository, ProbeRepository>();
            builder.Services.AddTransient<IMeasurementRepository, MeasurementRepository>();
            

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            RoverGenerator.Initialize(builder.Services);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            else
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.MapGroup("/rover").RoverGroup();
            app.MapGroup("/tasks").RoverTaskGroup();
            app.MapGroup("/probe").ProbeGroup();
            app.MapGroup("/measurement").MeasurementGroup();
            
            app.Run();
        }
    }
}