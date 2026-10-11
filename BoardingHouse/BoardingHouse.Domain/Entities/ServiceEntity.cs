namespace BoardingHouse.Domain.Entities;

public sealed class ServiceEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Unit { get; set; } = null!;

    public string CalculationType { get; set; } = null!;

    public decimal DefaultUnitPrice { get; set; }

    public bool IsActive { get; set; }
}
