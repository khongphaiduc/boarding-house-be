using BoardingHouse.Application.Rooms;
using Microsoft.Extensions.DependencyInjection;

namespace BoardingHouse.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IRoomService, RoomService>();
        return services;
    }
}
