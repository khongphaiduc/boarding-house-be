namespace BoardingHouse.Infrastructure.Models;

public partial class Service
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Unit { get; set; } = null!;

    public string CalculationType { get; set; } = null!;

    public decimal DefaultUnitPrice { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();

    public virtual ICollection<MeterReading> MeterReadings { get; set; } = new List<MeterReading>();

    public virtual ICollection<RoomService> RoomServices { get; set; } = new List<RoomService>();
}
