namespace BoardingHouse.Infrastructure.Models;

public partial class Floor
{
    public Guid Id { get; set; }

    public int FloorNumber { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
