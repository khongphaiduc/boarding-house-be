namespace BoardingHouse.Infrastructure.Models;

public partial class Resident
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string? IdentityNumber { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? PermanentAddress { get; set; }

    public string? EmergencyContactName { get; set; }

    public string? EmergencyContactPhone { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();

    public virtual User User { get; set; } = null!;
}
