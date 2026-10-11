using BoardingHouse.Application.Abstractions.Persistence;
using BoardingHouse.Domain.Entities;
using BoardingHouse.Infrastructure.Mappings;
using Microsoft.EntityFrameworkCore;

namespace BoardingHouse.Infrastructure.Persistence.Repositories;

internal sealed class RoomRepository(BoardingHouseDbContext context) : IRoomRepository
{
    public Task<int> CountAsync(CancellationToken cancellationToken = default) =>
        context.Rooms.CountAsync(cancellationToken);

    public async Task<IReadOnlyList<RoomEntity>> GetPageAsync(
        int pageNumber, int pageSize, CancellationToken cancellationToken = default) =>
        await context.Rooms.AsNoTracking()
            .OrderBy(room => room.RoomNumber).ThenBy(room => room.Id)
            .Skip(checked((pageNumber - 1) * pageSize))
            .Take(pageSize)
            .Select(RoomMapping.ToEntity)
            .ToListAsync(cancellationToken);

    public Task<RoomEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Rooms.AsNoTracking()
            .Where(room => room.Id == id)
            .Select(RoomMapping.ToEntity)
            .SingleOrDefaultAsync(cancellationToken);
}
