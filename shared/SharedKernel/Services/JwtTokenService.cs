using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SharedKernel.Services;

public static class JwtTokenService
{
    public static string CreateJwtToken(List<Claim> claims, string secret, string issuer, string audience)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static bool IsStandardJwtClaim(string claimType)
    {
        return claimType == "iss" || claimType == "aud" || claimType == "exp" ||
               claimType == "iat" || claimType == "nbf" || claimType == "jti";
    }

    public static IEnumerable<Claim> GetExtraClaims(IEnumerable<Claim> externalClaims)
    {
        var claims = new List<Claim>();

        foreach (var claim in externalClaims)
            if (!IsStandardJwtClaim(claim.Type))
                claims.Add(new Claim($"{claim.Type}", claim.Value));

        return claims;
    }
}
