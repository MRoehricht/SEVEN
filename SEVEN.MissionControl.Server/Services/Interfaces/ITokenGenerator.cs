using System.Security.Claims;

namespace SEVEN.MissionControl.Server.Services.Interfaces;

public interface ITokenGenerator {
    string GenerateToken(ClaimsIdentity claimsIdentity);
}