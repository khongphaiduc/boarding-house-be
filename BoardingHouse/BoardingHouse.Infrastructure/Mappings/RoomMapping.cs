using System.Linq.Expressions;
using BoardingHouse.Domain.Entities;
using BoardingHouse.Infrastructure.Models;

namespace BoardingHouse.Infrastructure.Mappings;

internal static class RoomMapping
{
    internal static readonly Expression<Func<Room, RoomEntity>> ToEntity = room => new RoomEntity
    {
        Id = room.Id,
        FloorId = room.FloorId,
        RoomNumber = room.RoomNumber,
        Area = room.Area,
        BasePrice = room.BasePrice,
        MaxOccupants = room.MaxOccupants,
        Description = room.Description,
        Status = room.Status,
        CreatedAt = room.CreatedAt
    };
}
