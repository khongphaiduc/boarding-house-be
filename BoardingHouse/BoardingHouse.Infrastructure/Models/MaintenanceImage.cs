namespace BoardingHouse.Infrastructure.Models;

public partial class MaintenanceImage
{
    public Guid Id { get; set; }

    public Guid MaintenanceRequestId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual MaintenanceRequest MaintenanceRequest { get; set; } = null!;
}
