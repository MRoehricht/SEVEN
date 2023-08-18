using System.Linq.Expressions;
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
    
    private static async Task<IResult> ExecuteDql(List<DqlToken> tokens, MissionControlContext context)
    {
        Expression<Func<Measurement, bool>> queryExpression = DqlQueryBuilder.BuildQuery<Measurement>(tokens);
        var result = context.Measurements.Where(queryExpression).ToList();
        return Results.Ok();
    }
}