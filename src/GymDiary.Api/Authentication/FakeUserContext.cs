using GymDiary.Application.Authentication;

namespace GymDiary.Api.Authentication;

// TODO: replace with HttpUserContext once real authentication is wired up.
public class FakeUserContext : IUserContext
{
    public bool IsAuthenticated => true;

    public string UserId => "507f1f77bcf86cd799439011";

    public string Email => "fake@gymdiary.local";
}
