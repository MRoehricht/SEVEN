using SEVEN.Core.Models;
using SEVEN.MissionControl.Api.Data.Contexts;
using SEVEN.MissionControl.Api.Models.Device;
using SEVEN.MissionControl.Api.Services.Device;

namespace SEVEN.MissionControl.Api.Features;

public static class DeviceEndpoint {
    public static RouteGroupBuilder DeviceGroup(this RouteGroupBuilder group) {
        group.MapPost("/dql", ExecuteDql).WithName("ExecuteDql").WithOpenApi();
        return group;
    }

    private static async Task<IResult> ExecuteDql(List<DqlToken> tokens, MissionControlContext context) {
        var builder = new DqlQueryBuilder();
        
        try {
            var groups = DqlQueryBuilder.GroupTokens(tokens);
            var predicate = builder.BuildExpression<Measurement>(groups);

            var query = context.Measurements.Where(predicate);
            var result = query.ToList();

            return Results.Ok(result);
        }
        catch (Exception ex) {
            return Results.BadRequest(ex.Message);
        }
    }
}