namespace BoardingHouse.Domain.Entities;

public sealed class RoomEntity
{
    public Guid Id { get; set; }

    public Guid FloorId { get; set; }

    public string RoomNumber { get; set; } = null!;

    public decimal Area { get; set; }

    public decimal BasePrice { get; set; }

    public int MaxOccupants { get; set; }

    public string? Description { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
