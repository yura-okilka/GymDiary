using GymDiary.Application.Authentication;

namespace GymDiary.Api.Authentication;

public class HttpUserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    private HttpContext HttpContext => httpContextAccessor.HttpContext ?? throw new Exception("HttpContext is not available");

    public bool IsAuthenticated => HttpContext.User.Identity?.IsAuthenticated ?? false;

    public string UserId => HttpContext.User.GetUserId();

    public string Email => HttpContext.User.GetUserEmail();
}
