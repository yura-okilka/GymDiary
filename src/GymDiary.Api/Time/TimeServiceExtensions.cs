using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GymDiary.Api.Time;

public static class TimeServiceExtensions
{
    public static IServiceCollection AddGymDiaryTime(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);

        return services;
    }
}
