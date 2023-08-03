using Microsoft.EntityFrameworkCore;
using SEVEN.MissionControl.Api.Data.Contexts;
using SEVEN.MissionControl.Api.Data.Generators;
using SEVEN.MissionControl.Api.Data.Repositories;
using SEVEN.MissionControl.Api.Data.Repositories.Interfaces;
using SEVEN.MissionControl.Api.Features;
using SEVEN.MissionControl.Api.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MissionControlContext>(optionsAction =>
{
    var postgresHost = builder.Configuration["DB_HOST"];
    var postgresPort = builder.Configuration["DB_PORT"];
    var postgresDatabase = builder.Configuration["DB_DB"];
    var postgresUser = builder.Configuration["DB_USER"];
    var postgresPassword = builder.Configuration["DB_PASSWORD"];
    optionsAction.UseNpgsql($"host={postgresHost};port={postgresPort};database={postgresDatabase};username={postgresUser};password={postgresPassword};");
});

var sevenOptions = new SEVENOptions();
builder.Configuration.Bind(sevenOptions);
builder.Services.Configure<SEVENOptions>(builder.Configuration);

builder.Services.AddTransient<IRoverTaskRepository, RoverTaskRepository>();
builder.Services.AddTransient<IProbeRepository, ProbeRepository>();
builder.Services.AddTransient<IMeasurementRepository, MeasurementRepository>();

RoverGenerator.Initialize(builder.Services);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGroup("/rover").RoverGroup();
app.MapGroup("/tasks").RoverTaskGroup();
app.MapGroup("/probe").ProbeGroup();
app.MapGroup("/measurement").MeasurementGroup();
app.Run();