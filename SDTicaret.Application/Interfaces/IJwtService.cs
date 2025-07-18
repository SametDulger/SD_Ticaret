using SDTicaret.Core;
using SDTicaret.Core.Entities;

namespace SDTicaret.Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    System.Security.Claims.ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
} 
