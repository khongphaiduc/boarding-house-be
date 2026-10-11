namespace BoardingHouse.Domain.Entities;

public sealed class MeterReadingEntity
{
    public Guid Id { get; set; }

    public Guid RoomId { get; set; }

    public Guid ServiceId { get; set; }

    public DateOnly BillingMonth { get; set; }

    public decimal PreviousReading { get; set; }

    public decimal CurrentReading { get; set; }

    public DateTime RecordedAt { get; set; }
}
