using System.Security.Claims;
using SEVEN.MissionControl.Server.Data.Contexts;
using OpenIddict.Abstractions;
using SEVEN.Core.Models;
using SEVEN.MissionControl.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Client.AspNetCore;
using OpenIddict.Client.WebIntegration;

namespace SEVEN.MissionControl.Server.API.Endpoints;

public static class AuthenticationEndpoint
{
    private static UserInfo _currentUser = null;
    
    public static RouteGroupBuilder AuthenticationGroup(this RouteGroupBuilder group)
    {
        group.MapGet("/", GetProbeToken).WithName("GetProbeToken").WithOpenApi();
        group.MapGet("/challenge/{redirectUrl}", HandleChallenge).WithName("HandleChallenge").WithOpenApi();
        group.MapGet("/me/", GetMe).WithName("GetMe").WithOpenApi();
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

    private static IResult GetMe(HttpContext context, ClaimsPrincipal user)
    {
        var a = user.GetClaim(OpenIddictConstants.Claims.Subject);

        if (Guid.TryParse(a, out var userGuid) && _currentUser != null && _currentUser.Id == userGuid)
        {
            return Results.Ok(_currentUser);
        }
        return Results.Ok(new UserInfo{Info = a});
    }

    public class UserInfo
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Info { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string AvatarUrl  { get; set; }
        public string Roles { get; set; }
    }
    
    
    private static async Task<IResult> HandleGithub(HttpContext context) {
        var result = await context.AuthenticateAsync(OpenIddictClientAspNetCoreDefaults.AuthenticationScheme);

        if (result.Succeeded == false) {
            return Results.Unauthorized();
            
        }

        var username = result.Principal.FindFirst("login")?.Value;
        var avatarUrl = result.Principal.FindFirst("avatar_url")?.Value;

        if (username == null) {
            return Results.Unauthorized();
            
        }

        _currentUser = new UserInfo
        {
            Username = username,
            DisplayName = username,
            AvatarUrl = avatarUrl ?? string.Empty
        };
        
/*
        var user = await _db.Users.SingleOrDefaultAsync(x => x.Username == username, c);

        if (user == null) {
            user = new Entities.User {
                Username = username,
                DisplayName = username,
                AvatarUrl = avatarUrl ?? string.Empty,
                Roles = string.Join(",", new List<string> {
                    UserRoles.Member
                })
            };

            await _db.Users.AddAsync(user, c);
            await _db.SaveChangesAsync(c);
        }
*/
        var identity = new ClaimsIdentity(
            authenticationType: OpenIddictClientWebIntegrationConstants.Providers.GitHub,
            nameType: OpenIddictConstants.Claims.Name,
            roleType: OpenIddictConstants.Claims.Role);

        identity.AddClaim(OpenIddictConstants.Claims.Subject, _currentUser.Id.ToString() as string);
        identity.AddClaim(OpenIddictConstants.Claims.Name, _currentUser.Username as string);
        //identity.AddClaim(OpenIddictConstants.Claims.Role, user.Roles.ToString() as string);

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

