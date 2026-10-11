namespace BoardingHouse.Domain.Entities;

public sealed class InvoiceEntity
{
    public Guid Id { get; set; }

    public string InvoiceCode { get; set; } = null!;

    public Guid ContractId { get; set; }

    public DateOnly BillingMonth { get; set; }

    public DateOnly IssueDate { get; set; }

    public DateOnly DueDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
