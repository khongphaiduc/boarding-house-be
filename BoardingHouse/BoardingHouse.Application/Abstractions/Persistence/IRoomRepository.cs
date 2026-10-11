using BoardingHouse.Domain.Entities;

namespace BoardingHouse.Application.Abstractions.Persistence;

public interface IRoomRepository
{
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RoomEntity>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<RoomEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
