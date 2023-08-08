using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace SEVEN.MissionControl.Api.AuthenticationSchemes;

public class ApiKeyAuthenticationScheme : AuthenticationHandler<AuthenticationSchemeOptions> {
    internal const string SchemeName = "seven-api-key";
    internal const string KeyHeaderName = "x-seven-key";
    private const string ProbeHeaderName = "x-probe-id";
    private readonly string _apiKey;

    public ApiKeyAuthenticationScheme(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder,
        ISystemClock clock, IConfiguration configuration) : base(options, logger, encoder, clock) {
        _apiKey = configuration["API_KEY"] ?? throw new InvalidOperationException("Seven api-key not set in appsettings.json or environment variables");
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync() {
        Request.Headers.TryGetValue(KeyHeaderName, out var extractedApiKey);
        Request.Headers.TryGetValue(ProbeHeaderName, out var extractedProbeId);

        if (!IsPublicEndpoint() && !extractedApiKey.Equals(_apiKey))
            return Task.FromResult(AuthenticateResult.Fail("Invalid seven-key"));

        var probeId = extractedProbeId.ToString() == string.Empty ? "Generic" : extractedProbeId.ToString();
        
        var identity = new ClaimsIdentity(
            claims: new[] {
                new Claim("ProbeId", probeId)
            },
            authenticationType: Scheme.Name);

        var principal = new GenericPrincipal(identity, roles: null);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private bool IsPublicEndpoint() => Context
        .GetEndpoint()?
        .Metadata.OfType<AllowAnonymousAttribute>()
        .Any() is null or true;
}