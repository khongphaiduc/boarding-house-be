using BoardingHouse.Application.Common.Models;

namespace BoardingHouse.Application.Rooms;

public interface IRoomService
{
    Task<PagedResult<RoomDto>> GetPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<RoomDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
