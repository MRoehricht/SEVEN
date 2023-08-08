using SEVEN.Core.Models;
using SEVEN.MissionControl.Api.Data.Contexts;
using SEVEN.MissionControl.Api.Data.Repositories.Interfaces;

namespace SEVEN.MissionControl.Api.Features;

public static class ProbeEndpoint
{
    public static RouteGroupBuilder ProbeGroup(this RouteGroupBuilder group)
    {
        group.MapGet("/{id}", GetProbe).WithName("GetProbe").WithOpenApi();
        group.MapPost("/", CreateProbe).WithName("CreateProbe").WithOpenApi();
        group.MapPut("/", UpdateProbe).WithName("UpdateProbe").WithOpenApi();
        group.MapDelete("/", RemoveProbe).WithName("RemoveProbe").WithOpenApi();

        return group;
    }

    private static async Task<IResult> GetProbe(Guid id, HttpContext httpContext, MissionControlContext context)
    {
        var probe = await context.Probes.FindAsync(id);
        return probe != null ? Results.Ok(probe) : Results.NotFound();
    }

    private static async Task<IResult> CreateProbe(Probe probe, IProbeRepository repository)
    {
        var createdProbe = await repository.CreateProbe(probe);
        return Results.Created($"/probe/{createdProbe.Id}", createdProbe);
    }

    private static async Task<IResult> UpdateProbe(Probe probe, IProbeRepository repository)
    {
        var updatedProbe = await repository.UpdateProbe(probe);
        return updatedProbe is null ? Results.NotFound() : Results.Created($"/probe/{updatedProbe.Id}", updatedProbe);
    }

    private static async Task<IResult> RemoveProbe(Guid id, IProbeRepository repository)
    {
        var isProbeDeleted = await repository.RemoveProbe(id);
        return isProbeDeleted ? Results.NoContent() : Results.NotFound();
    }
}