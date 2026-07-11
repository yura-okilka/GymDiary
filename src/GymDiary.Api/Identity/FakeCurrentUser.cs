using GymDiary.Application.Identity;

namespace GymDiary.Api.Identity;

// TODO: replace with HttpCurrentUser once real authentication is wired up.
public class FakeCurrentUser : ICurrentUser
{
    public Guid Id => Guid.Parse("018f1f77-bcf8-7cd7-9943-9011507f1f77");

    public string Email => "fake@gymdiary.local";
}
