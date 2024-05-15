using SEVEN.Core.Models;
using SEVEN.MissionControl.Api.Data.Contexts;
using SEVEN.MissionControl.Api.Models.Device;
using SEVEN.MissionControl.Api.Services.Device;

namespace SEVEN.MissionControl.Api.Features;

public static class DeviceEndpoint {
    public static RouteGroupBuilder DeviceGroup(this RouteGroupBuilder group) {
        group.MapPost("/dql", ExecuteDql).WithName("ExecuteDql").WithOpenApi();
        group.MapGet("/dql/properties", GetMeasurementProperties).WithName("GetMeasurementProperties").WithOpenApi();
        return group;
    }

    private static Task<IResult> ExecuteDql(List<DqlToken> tokens, MissionControlContext context) {
        var builder = new DqlQueryBuilder();

        IQueryable<Measurement> measurementQuery;

        try {
            if (!tokens.Any()) {
                measurementQuery = context.Measurements;
            }
            else {
                var groups = DqlQueryBuilder.GroupTokens(tokens);
                var predicate = builder.BuildExpression<Measurement>(groups);
                measurementQuery = context.Measurements.Where(predicate);
            }

            var result = measurementQuery.ToList();
            return Task.FromResult(Results.Ok(result));
        }
        catch (Exception ex) {
            return Task.FromResult(Results.BadRequest(ex.Message));
        }
    }

    private static Task<IResult> GetMeasurementProperties() {
        var properties = typeof(Measurement).GetProperties().Select(p => p.Name).ToArray();

        var dqlProperties = properties.Select(property => new DqlProperties {
                PropertyName = property,
                PropertyValues = new string[] {
                }
            })
            .ToList();

        return Task.FromResult(Results.Ok(dqlProperties));
    }
}