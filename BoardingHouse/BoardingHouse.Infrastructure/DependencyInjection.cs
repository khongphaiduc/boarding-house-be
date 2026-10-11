using BoardingHouse.Application.Abstractions.Persistence;
using BoardingHouse.Infrastructure.Persistence;
using BoardingHouse.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BoardingHouse.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Configure ConnectionStrings:DefaultConnection using ConnectionStrings__DefaultConnection in .env or environment variables before starting the API.");
        }

        services.AddDbContext<BoardingHouseDbContext>(options => options.UseSqlServer(
            connectionString, sql => sql.EnableRetryOnFailure(3)));
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<BoardingHouseDbContext>());
        return services;
    }
}
