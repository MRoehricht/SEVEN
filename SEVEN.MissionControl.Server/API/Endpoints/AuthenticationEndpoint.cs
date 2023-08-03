using System.Security.Claims;
using SEVEN.MissionControl.Server.Data.Contexts;
using OpenIddict.Abstractions;
using SEVEN.Core.Models;
using SEVEN.MissionControl.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Client.AspNetCore;
using OpenIddict.Client.WebIntegration;

namespace SEVEN.MissionControl.Server.API.Endpoints;

public static class AuthenticationEndpoint
{
    private static SevenUser? _currentUser = null;
    
    public static RouteGroupBuilder AuthenticationGroup(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetProbeToken).WithName("GetProbeToken").WithOpenApi();
        group.MapGet("/challenge/{redirectUrl}", HandleChallenge).WithName("HandleChallenge").WithOpenApi();
        group.MapGet("/me/", GetMe).WithName("GetMe").WithOpenApi();
        group.MapGet("/logout/", Logout).WithName("Logout").WithOpenApi();
        group.MapGet("/webhook/oauth/github/", HandleGithub).WithName("HandleGithub").WithOpenApi();
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

    private static async Task<IResult>  HandleChallenge(string? redirectUrl, HttpContext context)
    {
        var authenticationProperties = new AuthenticationProperties(new Dictionary<string, string?> {
            [OpenIddictClientAspNetCoreConstants.Properties.ProviderName] = OpenIddictClientWebIntegrationConstants.Providers.GitHub,
        }) {
            RedirectUri = redirectUrl
        };

        context.Response.OnStarting(() => {
            context.Response.StatusCode = 302;
            return Task.CompletedTask;
        });

        await context.ChallengeAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme, authenticationProperties);
        return Results.Ok();
        return Results.Redirect("/");
    }

    private static async Task<IResult> GetMe(MissionControlContext dbContext, HttpContext context, ClaimsPrincipal claims)
    {
        var subject = claims.GetClaim(OpenIddictConstants.Claims.Subject);

        if (Guid.TryParse(subject, out var userGuid))
        {
            var user = await dbContext.Users.FindAsync(userGuid);
            if (user is null)
            {
                return Results.Unauthorized();
            }
            
            return Results.Ok(user);
        }
        
        return Results.Unauthorized();
    }
    private static async Task<IResult> Logout(MissionControlContext dbContext, HttpContext context) {
        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        context.Response.Redirect("/");
        return Results.Redirect("/");
    }

    private static async Task<IResult> HandleGithub(MissionControlContext dbContext, HttpContext context) {
        var result = await context.AuthenticateAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);

        if (result.Succeeded == false) {
            return Results.Unauthorized();
            
        }

        var username = result.Principal.FindFirst("login")?.Value;
        var avatarUrl = result.Principal.FindFirst("avatar_url")?.Value;

        if (username == null) {
            return Results.Unauthorized();
            
        }

        _currentUser = new SevenUser
        {
            Username = username,
            DisplayName = username,
            AvatarUrl = avatarUrl ?? string.Empty
        };
        
        var user = await dbContext.Users.SingleOrDefaultAsync(x => x.Username == username);

        if (user == null) {
            user = new SevenUser{
                Username = username,
                DisplayName = username,
                AvatarUrl = avatarUrl ?? string.Empty,
                Roles = UserRoles.Member
            };

            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();
        }

        var identity = new ClaimsIdentity(
            authenticationType: OpenIddictClientWebIntegrationConstants.Providers.GitHub,
            nameType: OpenIddictConstants.Claims.Name,
            roleType: OpenIddictConstants.Claims.Role);

        identity.AddClaim(OpenIddictConstants.Claims.Subject, user.Id.ToString() as string);
        identity.AddClaim(OpenIddictConstants.Claims.Name, user?.Username);
        identity.AddClaim(OpenIddictConstants.Claims.Role, user?.Roles?.ToString());

        var authenticationProperties = new AuthenticationProperties {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60),
        };

        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), authenticationProperties);
        
        if (result.Properties.RedirectUri != null)
        {
            context.Response.Redirect(result.Properties.RedirectUri);
            return Results.Redirect(result.Properties.RedirectUri);
        }
        
        return Results.Unauthorized();
    }
}

