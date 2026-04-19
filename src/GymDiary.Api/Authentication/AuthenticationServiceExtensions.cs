using GymDiary.Application.Authentication;

namespace GymDiary.Api.Authentication;

public static class AuthenticationServiceExtensions
{
    public static IServiceCollection AddGymDiaryAuthentication(this IServiceCollection services)
    {
        services
            .AddHttpContextAccessor()
            .AddTransient<IUserContext, HttpUserContext>();

        return services;
    }
}
