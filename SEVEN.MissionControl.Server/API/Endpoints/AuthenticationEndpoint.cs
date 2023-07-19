using System.Security.Claims;
using SEVEN.MissionControl.Server.Data.Contexts;
using OpenIddict.Abstractions;
using SEVEN.Core.Models;
using SEVEN.MissionControl.Server.Services.Interfaces;

namespace SEVEN.MissionControl.Server.API.Endpoints;

public static class AuthenticationEndpoint
{
    public static RouteGroupBuilder AuthenticationGroup(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetProbeToken).WithName("GetProbeToken").WithOpenApi();
        return group;
    }

    private static async Task<IResult> GetProbeToken(Guid id, HttpContext httpContext, MissionControlContext context, ITokenGenerator tokenGenerator)
    {
        var probe = await context.Probes.FindAsync(id);
        if (probe is null) return Results.NotFound();
        
        var identity = new ClaimsIdentity(
            authenticationType: "JWT",
            nameType: OpenIddictConstants.Claims.Name,
            roleType: OpenIddictConstants.Claims.Role);

        identity.AddClaim(OpenIddictConstants.Claims.Subject, probe.Id.ToString());
        identity.AddClaim(OpenIddictConstants.Claims.Name, probe.Name);
        var jwtToken = tokenGenerator.GenerateToken(identity);

        var result = new ProbeToken
        {
            ProbeId = id,
            DateOfExpiry = DateTime.UtcNow.AddMonths(1),
            Token = jwtToken,
            Type = "Bearer"
        };

        return Results.Ok(result);
    }
}

