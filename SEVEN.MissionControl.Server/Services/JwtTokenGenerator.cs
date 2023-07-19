using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SEVEN.MissionControl.Server.Options;
using SEVEN.MissionControl.Server.Services.Interfaces;

namespace SEVEN.MissionControl.Server.Services;

public class JwtTokenGenerator : ITokenGenerator {
private readonly SEVENOptions _options;

public JwtTokenGenerator(IOptions<SEVENOptions> options) {
    _options = options.Value;
}

public string GenerateToken(ClaimsIdentity claimsIdentity) {
    if (claimsIdentity == null) {
        throw new ArgumentException("Value cannot be null or whitespace.", nameof(claimsIdentity));
    }

    if (string.IsNullOrWhiteSpace(_options.PROBE_SECRET)) {
        throw new ArgumentException("Value cannot be null or whitespace.", nameof(_options.PROBE_SECRET));
    }

    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.PROBE_SECRET));
    var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

    var tokenDescriptor = new SecurityTokenDescriptor {
        Issuer = "SEVEN",
        Audience = "SEVEN",
        Expires = DateTime.UtcNow.AddMonths(1),
        SigningCredentials = signingCredentials,
        Claims = claimsIdentity.Claims.ToDictionary(c => c.Type, c => (object)c.Value),
        IssuedAt = DateTime.UtcNow,
    };

    var tokenHandler = new JwtSecurityTokenHandler();
    var securityToken = tokenHandler.CreateToken(tokenDescriptor);

    return tokenHandler.WriteToken(securityToken);
}
}