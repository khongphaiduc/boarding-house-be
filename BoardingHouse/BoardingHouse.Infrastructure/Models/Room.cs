namespace BoardingHouse.Infrastructure.Models;

public partial class Room
{
    public Guid Id { get; set; }

    public Guid FloorId { get; set; }

    public string RoomNumber { get; set; } = null!;

    public decimal Area { get; set; }

    public decimal BasePrice { get; set; }

    public int MaxOccupants { get; set; }

    public string? Description { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Contract? Contract { get; set; }

    public virtual Floor Floor { get; set; } = null!;

    public virtual ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();

    public virtual ICollection<MeterReading> MeterReadings { get; set; } = new List<MeterReading>();

    public virtual RoomImage? RoomImage { get; set; }

    public virtual ICollection<RoomService> RoomServices { get; set; } = new List<RoomService>();
}
