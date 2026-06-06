using GymDiary.Application.Identity;

namespace GymDiary.Api.Identity;

// TODO: replace with HttpCurrentUser once real authentication is wired up.
public class FakeCurrentUser : ICurrentUser
{
    public string Id => "507f1f77bcf86cd799439011";

    public string Email => "fake@gymdiary.local";
}
