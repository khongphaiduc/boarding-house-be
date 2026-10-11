namespace BoardingHouse.Application.Rooms;

public sealed record RoomDto(
    Guid Id,
    Guid FloorId,
    string RoomNumber,
    decimal Area,
    decimal BasePrice,
    int MaxOccupants,
    string? Description,
    string Status);
