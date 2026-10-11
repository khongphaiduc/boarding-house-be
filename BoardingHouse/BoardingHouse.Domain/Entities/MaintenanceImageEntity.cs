namespace BoardingHouse.Domain.Entities;

public sealed class MaintenanceImageEntity
{
    public Guid Id { get; set; }

    public Guid MaintenanceRequestId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
