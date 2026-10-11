namespace BoardingHouse.Domain.Entities;

public sealed class FloorEntity
{
    public Guid Id { get; set; }

    public int FloorNumber { get; set; }

    public string Name { get; set; } = null!;
}
