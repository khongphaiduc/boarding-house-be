namespace BoardingHouse.Infrastructure.Models;

public partial class Contract
{
    public Guid Id { get; set; }

    public string ContractCode { get; set; } = null!;

    public Guid RoomId { get; set; }

    public Guid ResidentId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public decimal MonthlyRent { get; set; }

    public decimal DepositAmount { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? SignedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual Resident Resident { get; set; } = null!;

    public virtual Room Room { get; set; } = null!;
}
