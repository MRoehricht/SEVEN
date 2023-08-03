using SEVEN.MissionControl.Api.Data.Contexts;

namespace SEVEN.MissionControl.Api.Features;

public static class RoverEndpoint
{
    public static RouteGroupBuilder RoverGroup(this RouteGroupBuilder group)
    {
        group.MapGet("/{id}", GetRover).WithName("GetRover").WithOpenApi();
        ;

        return group;
    }

    private static async Task<IResult> GetRover(Guid id, HttpContext httpContext, MissionControlContext context)
    {
        var rover = await context.Rovers.FindAsync(id);
        if (rover != null)
        {
            context.Entry(rover).Collection(b => b.Tasks).Load();
            return Results.Ok(rover);
        }

        return Results.NotFound();
    }
}