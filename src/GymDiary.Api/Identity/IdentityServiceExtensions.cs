using GymDiary.Application.Identity;

namespace GymDiary.Api.Identity;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddGymDiaryIdentity(this IServiceCollection services)
    {
        services
            .AddHttpContextAccessor()
            // TODO: swap FakeCurrentUser for HttpCurrentUser once real authentication is wired up.
            .AddTransient<ICurrentUser, FakeCurrentUser>();

        return services;
    }
}
