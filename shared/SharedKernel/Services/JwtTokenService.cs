using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SharedKernel.Services;

/// <summary>
/// Service for creating and managing JWT tokens
/// </summary>
public static class JwtTokenService
{
    /// <summary>
    /// Creates a JWT token with the specified claims and configuration
    /// </summary>
    /// <param name="claims">List of claims to include in the token</param>
    /// <param name="secret">Secret key used for signing the token</param>
    /// <param name="issuer">Token issuer identifier</param>
    /// <param name="audience">Token audience identifier</param>
    /// <returns>A JWT token string</returns>
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

    /// <summary>
    /// Determines if a claim type is a standard JWT claim that should not be included in custom claims
    /// </summary>
    /// <param name="claimType">The claim type to check</param>
    /// <returns>True if the claim type is a standard JWT claim, false otherwise</returns>
    private static bool IsStandardJwtClaim(string claimType)
    {
        return claimType == "iss" || claimType == "aud" || claimType == "exp" ||
               claimType == "iat" || claimType == "nbf" || claimType == "jti";
    }

    /// <summary>
    /// Extracts non-standard claims from a collection of external claims, filtering out JWT standard claims
    /// </summary>
    /// <param name="externalClaims">Collection of claims to filter</param>
    /// <returns>Collection of non-standard claims that can be safely included in custom tokens</returns>
    public static IEnumerable<Claim> GetExtraClaims(IEnumerable<Claim> externalClaims)
    {
        var claims = new List<Claim>();

        foreach (var claim in externalClaims)
            if (!IsStandardJwtClaim(claim.Type))
                claims.Add(new Claim($"{claim.Type}", claim.Value));

        return claims;
    }
}
