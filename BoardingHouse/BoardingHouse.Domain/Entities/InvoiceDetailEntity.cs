namespace BoardingHouse.Domain.Entities;

public sealed class InvoiceDetailEntity
{
    public Guid Id { get; set; }

    public Guid InvoiceId { get; set; }

    public Guid? ServiceId { get; set; }

    public string Description { get; set; } = null!;

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? Amount { get; set; }
}
