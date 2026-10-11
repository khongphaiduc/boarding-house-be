namespace BoardingHouse.Domain.Entities;

public sealed class ResidentEntity
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
}
