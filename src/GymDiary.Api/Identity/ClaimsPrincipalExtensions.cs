using System.Security.Claims;

using Microsoft.IdentityModel.JsonWebTokens;

namespace GymDiary.Api.Identity;

public static class ClaimsPrincipalExtensions
{
    extension(ClaimsPrincipal principal)
    {
        public string FindFirstOrThrow(string type)
        {
            return principal.FindFirst(type)?.Value ?? throw new Exception($"{type} claim was not found");
        }

        public T FindFirstOrThrow<T>(string type) where T : IParsable<T>
        {
            var value = principal.FindFirstOrThrow(type);

            return T.TryParse(value, null, out var result)
                ? result
                : throw new Exception($"{type} claim is not a valid {typeof(T).Name}");
        }

        public Guid GetUserId() => principal.FindFirstOrThrow<Guid>(JwtRegisteredClaimNames.Sub);

        public string GetUserEmail() => principal.FindFirstOrThrow(JwtRegisteredClaimNames.Email);
    }
}
