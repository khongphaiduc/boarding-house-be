using BoardingHouse.Application;
using BoardingHouse.Application.Abstractions.Persistence;
using BoardingHouse.Application.Rooms;
using BoardingHouse.Domain.Entities;
using BoardingHouse.Infrastructure;
using BoardingHouse.Infrastructure.Mappings;
using Microsoft.EntityFrameworkCore;
using BoardingHouse.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BoardingHouse.Tests;

public sealed class ArchitectureTests
{
    [Fact]
    public void Domain_and_application_do_not_reference_outer_layers_or_entity_framework()
    {
        var domainDependencies = typeof(RoomEntity).Assembly.GetReferencedAssemblies();
        Assert.DoesNotContain(domainDependencies, assembly => assembly.Name!.StartsWith("BoardingHouse."));
        Assert.DoesNotContain(domainDependencies, assembly => assembly.Name!.StartsWith("Microsoft.EntityFrameworkCore"));

        var applicationDependencies = typeof(IRoomService).Assembly.GetReferencedAssemblies();
        Assert.DoesNotContain(applicationDependencies, assembly =>
            assembly.Name is "BoardingHouse.Infrastructure" or "BoardingHouse.API" ||
            assembly.Name!.StartsWith("Microsoft.EntityFrameworkCore"));
    }

    [Fact]
    public void Infrastructure_registers_database_models_and_shares_context_with_unit_of_work()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = "Server=localhost;Database=BoardingHouseDB;Trusted_Connection=True;TrustServerCertificate=True;"
        }).Build();
        var services = new ServiceCollection().AddApplication().AddInfrastructure(configuration);
        using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true, ValidateScopes = true });
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BoardingHouseDbContext>();

        Assert.IsType<RoomService>(scope.ServiceProvider.GetRequiredService<IRoomService>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IRoomRepository>());
        Assert.Same(context, scope.ServiceProvider.GetRequiredService<IUnitOfWork>());
        Assert.Equal(17, context.Model.GetEntityTypes().Count());
        var domainTypes = typeof(RoomEntity).Assembly.GetExportedTypes()
            .Where(type => type.Namespace == "BoardingHouse.Domain.Entities")
            .ToDictionary(type => type.Name);
        Assert.Equal(17, domainTypes.Count);
        Assert.All(domainTypes.Values, type => Assert.EndsWith("Entity", type.Name));
        Assert.All(context.Model.GetEntityTypes(), model =>
        {
            Assert.Equal(typeof(BoardingHouseDbContext).Assembly, model.ClrType.Assembly);
            Assert.Equal("BoardingHouse.Infrastructure.Models", model.ClrType.Namespace);
            Assert.True(domainTypes.TryGetValue(model.ClrType.Name + "Entity", out var domainType));
            Assert.NotEqual(model.ClrType, domainType);
        });
        Assert.Equal("Microsoft.EntityFrameworkCore.SqlServer", context.Database.ProviderName);
    }

    [Fact]
    public void Room_projection_translates_to_SQL_for_database_model_and_returns_domain_entity()
    {
        var options = new DbContextOptionsBuilder<BoardingHouseDbContext>()
            .UseSqlServer("Server=localhost;Database=BoardingHouseDB;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;
        using var context = new BoardingHouseDbContext(options);
        var query = context.Rooms.AsNoTracking()
            .OrderBy(room => room.RoomNumber).ThenBy(room => room.Id)
            .Skip(20).Take(20)
            .Select(RoomMapping.ToEntity);

        Assert.Equal(typeof(RoomEntity), query.ElementType);
        var sql = query.ToQueryString();
        Assert.Contains("[Rooms]", sql);
        Assert.Contains("OFFSET", sql);
        Assert.DoesNotContain("RoomEntity", sql);
    }

    [Fact]
    public void Missing_connection_configuration_fails_with_clear_message()
    {
        var configuration = new ConfigurationBuilder().Build();
        var exception = Assert.Throws<InvalidOperationException>(() =>
            new ServiceCollection().AddInfrastructure(configuration));
        Assert.Contains("ConnectionStrings:DefaultConnection", exception.Message);
    }
}
