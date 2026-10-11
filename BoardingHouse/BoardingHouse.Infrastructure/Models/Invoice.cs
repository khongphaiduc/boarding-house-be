namespace BoardingHouse.Infrastructure.Models;

public partial class Invoice
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

    public virtual Contract Contract { get; set; } = null!;

    public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
