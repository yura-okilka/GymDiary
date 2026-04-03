using System.Security.Claims;

using Microsoft.IdentityModel.JsonWebTokens;

namespace GymDiary.Api.CSharp.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static string FindFirstOrThrow(this ClaimsPrincipal principal, string type)
    {
        return principal.FindFirst(type)?.Value ?? throw new Exception($"{type} claim was not found");
    }

    public static string GetUserId(this ClaimsPrincipal principal) => principal.FindFirstOrThrow(JwtRegisteredClaimNames.Sub);

    public static string GetUserEmail(this ClaimsPrincipal principal) => principal.FindFirstOrThrow(JwtRegisteredClaimNames.Email);
}
