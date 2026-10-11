using BoardingHouse.Application.Abstractions.Persistence;
using BoardingHouse.Application.Common.Exceptions;
using BoardingHouse.Application.Common.Models;
using BoardingHouse.Domain.Entities;

namespace BoardingHouse.Application.Rooms;

public sealed class RoomService(IRoomRepository repository) : IRoomService
{
    public async Task<PagedResult<RoomDto>> GetPageAsync(
        int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize is < 1 or > 100 ||
            (long)(pageNumber - 1) * pageSize > int.MaxValue)
        {
            throw new ValidationException("pageNumber must be >= 1, pageSize must be between 1 and 100, and the pagination offset must fit in Int32.");
        }

        var totalCount = await repository.CountAsync(cancellationToken);
        var rooms = await repository.GetPageAsync(pageNumber, pageSize, cancellationToken);
        return new PagedResult<RoomDto>(rooms.Select(Map).ToArray(), pageNumber, pageSize, totalCount);
    }

    public async Task<RoomDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var room = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Room '{id}' was not found.");
        return Map(room);
    }

    private static RoomDto Map(RoomEntity room) => new(
        room.Id, room.FloorId, room.RoomNumber, room.Area, room.BasePrice,
        room.MaxOccupants, room.Description, room.Status);
}
