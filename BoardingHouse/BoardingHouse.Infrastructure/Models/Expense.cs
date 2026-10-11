namespace BoardingHouse.Infrastructure.Models;

public partial class Expense
{
    public Guid Id { get; set; }

    public Guid? MaintenanceRequestId { get; set; }

    public string Description { get; set; } = null!;

    public string Category { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateOnly ExpenseDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual MaintenanceRequest? MaintenanceRequest { get; set; }
}
