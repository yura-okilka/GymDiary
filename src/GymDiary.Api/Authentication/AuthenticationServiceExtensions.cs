using GymDiary.Application.Authentication;

namespace GymDiary.Api.Authentication;

public static class AuthenticationServiceExtensions
{
    public static IServiceCollection AddGymDiaryAuthentication(this IServiceCollection services)
    {
        services
            .AddHttpContextAccessor()
            // TODO: swap FakeUserContext for HttpUserContext once real authentication is wired up.
            .AddTransient<IUserContext, FakeUserContext>();

        return services;
    }
}
