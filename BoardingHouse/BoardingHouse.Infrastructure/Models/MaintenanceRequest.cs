namespace BoardingHouse.Infrastructure.Models;

public partial class MaintenanceRequest
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

    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    public virtual ICollection<MaintenanceImage> MaintenanceImages { get; set; } = new List<MaintenanceImage>();

    public virtual Resident Resident { get; set; } = null!;

    public virtual Room Room { get; set; } = null!;
}
