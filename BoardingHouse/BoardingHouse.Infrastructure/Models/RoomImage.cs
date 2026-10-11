namespace BoardingHouse.Infrastructure.Models;

public partial class RoomImage
{
    public Guid Id { get; set; }

    public Guid RoomId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool IsThumbnail { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Room Room { get; set; } = null!;
}
