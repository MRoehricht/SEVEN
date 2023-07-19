using SEVEN.Core.Models;
using SEVEN.MissionControl.Server.Data.Repositories.Interfaces;

namespace SEVEN.MissionControl.Server.API.Endpoints;

public static class MeasurementEndpoint
{
    public static RouteGroupBuilder MeasurementGroup(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetMeasurements).WithName("GetMeasurements").WithOpenApi();
        group.MapPost("/", PostMeasurement).WithName("PostMeasurement").RequireAuthorization().WithOpenApi();
        return group;
    }

    private static async Task<IResult> GetMeasurements(IMeasurementRepository repository)
    {
        var measurements = await repository.GetMeasurements();
        return Results.Ok(measurements);
    }

    private static async Task<IResult> PostMeasurement(Measurement measurement, IMeasurementRepository repository)
    {
        var dbMeasurement = await repository.CreateMeasurement(measurement);
        return dbMeasurement != null ? Results.Ok(dbMeasurement) : Results.NotFound();
    }
}