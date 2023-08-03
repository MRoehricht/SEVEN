using Microsoft.AspNetCore.Components.Authorization;
using OpenIddict.Abstractions;
using SEVEN.Core.Models;
using SEVEN.MissionControl.Server.Data.Contexts;
using SEVEN.MissionControl.Server.Services.Interfaces;

namespace SEVEN.MissionControl.Server.Services;

public class UserService : IUserService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly MissionControlContext _context;

    public UserService(AuthenticationStateProvider authenticationStateProvider, MissionControlContext context)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _context = context;
    }

    public async Task<SevenUser?> GetCurrentUser()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        if (authState.User.Identity is null || !authState.User.Identity.IsAuthenticated) return null;
        
        var subject = authState.User.GetClaim(OpenIddictConstants.Claims.Subject);
        if (!Guid.TryParse(subject, out var userGuid)) return null;
        
        var user = await _context.Users.FindAsync(userGuid);
        return user ?? null;

    }
}