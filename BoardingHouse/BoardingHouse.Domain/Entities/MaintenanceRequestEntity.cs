namespace BoardingHouse.Domain.Entities;

public sealed class MaintenanceRequestEntity
{
    public Guid Id { get; set; }

    public Guid RoomId { get; set; }

    public Guid ResidentId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Priority { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }
}
