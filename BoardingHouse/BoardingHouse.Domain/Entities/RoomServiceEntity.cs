namespace BoardingHouse.Domain.Entities;

public sealed class RoomServiceEntity
{
    public Guid Id { get; set; }

    public Guid RoomId { get; set; }

    public Guid ServiceId { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal DefaultQuantity { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public bool IsActive { get; set; }
}
