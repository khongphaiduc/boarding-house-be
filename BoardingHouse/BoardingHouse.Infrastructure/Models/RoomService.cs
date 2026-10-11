namespace BoardingHouse.Infrastructure.Models;

public partial class RoomService
{
    public Guid Id { get; set; }

    public Guid RoomId { get; set; }

    public Guid ServiceId { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal DefaultQuantity { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public bool IsActive { get; set; }

    public virtual Room Room { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
