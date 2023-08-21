using System.Security.Claims;
using OpenIddict.Abstractions;
using SEVEN.Core.Models;
using SEVEN.MissionControl.Api.Data.Repositories.Interfaces;
using SEVEN.MissionControl.Api.Services;
using SEVEN.MissionControl.Api.Services.ChartServices;

namespace SEVEN.MissionControl.Api.Features;

public static class MeasurementEndpoint
{
    public static RouteGroupBuilder MeasurementGroup(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetMeasurements).WithName("GetMeasurements").WithOpenApi();
        group.MapPost("/filter", GetFilteredMeasurements).WithName("GetFilteredMeasurements").WithOpenApi();
        group.MapGet("/lastMeasurement", GetLastMeasurement).WithName("GetLastMeasurement").WithOpenApi();
        group.MapGet("/create/{message}", CreateMessages).WithName("CreateMessages").WithOpenApi();
        group.MapPost("/", PostMeasurement).WithName("PostMeasurement").WithOpenApi();
        group.MapDelete("/", DeleteMeasurement).WithName("DeleteMeasurement").WithOpenApi();
        return group;
    }

    private static async Task<IResult> GetMeasurements(IMeasurementRepository repository)
    {
        var measurements = await repository.GetMeasurements();
        return Results.Ok(measurements); 
    }

    private static async Task<IResult> GetLastMeasurement(Guid probeId, IMeasurementRepository repository)
    {
        var measurement = await repository.GetLastMeasurement(probeId);
        return Results.Ok(measurement);
    }

    private static async Task<IResult> GetFilteredMeasurements(MeasurementFilter? filter, IMeasurementRepository repository)
    {
        var measurements = await repository.GetMeasurements();
        if (filter == null) return Results.Ok(measurements);

        if (filter.ProbeId.HasValue)
            measurements = measurements.Where(_ => _.ProbeId == filter.ProbeId.Value);

        if (filter.Year.HasValue)
            measurements = measurements.Where(_ => DateOnly.FromDateTime(_.Time).Year == filter.Year.Value);

        if (filter.Month.HasValue)
            measurements = measurements.Where(_ => DateOnly.FromDateTime(_.Time).Month == filter.Month.Value);

        if (filter.Day.HasValue)
            measurements = measurements.Where(_ => DateOnly.FromDateTime(_.Time).Day == filter.Day.Value);

        if (filter.Type.HasValue)
            measurements = measurements.Where(_ => _.MeasurementType == (filter.Type.Value));

        if (filter.ReduceData.HasValue && filter.ReduceData.Value)
            measurements = ReduceDataService.ReduceData(measurements);

        return Results.Ok(measurements.OrderBy(_ => _.Time));
    }

    private static async Task<IResult> CreateMessages(string? message, IMeasurementRepository repository)
    {
        var measurements = MeasurementMapperService.Map(message);
        foreach (var measurement in measurements)
        {
            await repository.CreateMeasurement(measurement);
        }

        return Results.Ok(measurements.Count());
    }

    private static async Task<IResult> PostMeasurement(Measurement measurement, IMeasurementRepository repository)
    {
        var dbMeasurement = await repository.CreateMeasurement(measurement);
        return dbMeasurement != null ? Results.Ok(dbMeasurement) : Results.NotFound();
    }
    
    private static async Task<IResult> DeleteMeasurement(Guid id, IMeasurementRepository repository)
    {
        var isMeasurementDeleted = await repository.DeleteMeasurement(id);
        return isMeasurementDeleted ? Results.NoContent() : Results.NotFound();
    }
    
}