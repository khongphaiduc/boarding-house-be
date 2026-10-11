namespace BoardingHouse.Infrastructure.Models;

public partial class MeterReading
{
    public Guid Id { get; set; }

    public Guid RoomId { get; set; }

    public Guid ServiceId { get; set; }

    public DateOnly BillingMonth { get; set; }

    public decimal PreviousReading { get; set; }

    public decimal CurrentReading { get; set; }

    public DateTime RecordedAt { get; set; }

    public virtual Room Room { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
