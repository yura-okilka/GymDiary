using GymDiary.Application.Identity;

namespace GymDiary.Api.Identity;

public class HttpCurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private HttpContext HttpContext => httpContextAccessor.HttpContext ?? throw new Exception("HttpContext is not available");

    public Guid Id => HttpContext.User.GetUserId();

    public string Email => HttpContext.User.GetUserEmail();
}
