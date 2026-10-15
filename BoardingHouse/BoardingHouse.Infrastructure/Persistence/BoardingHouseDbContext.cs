using BoardingHouse.Application.Abstractions.Persistence;
using BoardingHouse.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardingHouse.Infrastructure.Persistence;

public partial class BoardingHouseDbContext : DbContext, IUnitOfWork
{
    public BoardingHouseDbContext(DbContextOptions<BoardingHouseDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<Expense> Expenses { get; set; }

    public virtual DbSet<Floor> Floors { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoiceDetail> InvoiceDetails { get; set; }

    public virtual DbSet<MaintenanceImage> MaintenanceImages { get; set; }

    public virtual DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }

    public virtual DbSet<MeterReading> MeterReadings { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Resident> Residents { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomImage> RoomImages { get; set; }

    public virtual DbSet<RoomService> RoomServices { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasIndex(e => new { e.ResidentId, e.Status }, "IX_Contracts_Resident_Status");

            entity.HasIndex(e => e.ContractCode, "UQ_Contracts_Code").IsUnique();

            entity.HasIndex(e => e.RoomId, "UX_Contracts_OneActivePerRoom")
                .IsUnique()
                .HasFilter("([Status]='Active')");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.ContractCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.DepositAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MonthlyRent).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SignedAt).HasPrecision(0);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Resident).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.ResidentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Contracts_Residents");

            entity.HasOne(d => d.Room).WithOne(p => p.Contract)
                .HasForeignKey<Contract>(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Contracts_Rooms");
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasIndex(e => new { e.ExpenseDate, e.Category }, "IX_Expenses_Date_Category");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Category)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description).HasMaxLength(250);

            entity.HasOne(d => d.MaintenanceRequest).WithMany(p => p.Expenses)
                .HasForeignKey(d => d.MaintenanceRequestId)
                .HasConstraintName("FK_Expenses_Maintenance");
        });

        modelBuilder.Entity<Floor>(entity =>
        {
            entity.HasIndex(e => e.FloorNumber, "UQ_Floors_FloorNumber").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasIndex(e => new { e.Status, e.DueDate }, "IX_Invoices_Status_DueDate");

            entity.HasIndex(e => e.InvoiceCode, "UQ_Invoices_Code").IsUnique();

            entity.HasIndex(e => new { e.ContractId, e.BillingMonth }, "UQ_Invoices_Contract_Month").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.InvoiceCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Draft");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Contract).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.ContractId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Invoices_Contracts");
        });

        modelBuilder.Entity<InvoiceDetail>(entity =>
        {
            entity.HasIndex(e => e.InvoiceId, "IX_InvoiceDetails_InvoiceId");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Amount)
                .HasComputedColumnSql("(CONVERT([decimal](18,2),[Quantity]*[UnitPrice]))", true)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.Quantity).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceDetails)
                .HasForeignKey(d => d.InvoiceId)
                .HasConstraintName("FK_InvoiceDetails_Invoices");

            entity.HasOne(d => d.Service).WithMany(p => p.InvoiceDetails)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("FK_InvoiceDetails_Services");
        });

        modelBuilder.Entity<MaintenanceImage>(entity =>
        {
            entity.HasIndex(e => e.MaintenanceRequestId, "IX_MaintenanceImages_Request");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.ImageUrl).HasMaxLength(1000);

            entity.HasOne(d => d.MaintenanceRequest).WithMany(p => p.MaintenanceImages)
                .HasForeignKey(d => d.MaintenanceRequestId)
                .HasConstraintName("FK_MaintenanceImages_Requests");
        });

        modelBuilder.Entity<MaintenanceRequest>(entity =>
        {
            entity.HasIndex(e => new { e.RoomId, e.Status }, "IX_Maintenance_Room_Status");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CompletedAt).HasPrecision(0);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Priority)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Medium");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Resident).WithMany(p => p.MaintenanceRequests)
                .HasForeignKey(d => d.ResidentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Maintenance_Residents");

            entity.HasOne(d => d.Room).WithMany(p => p.MaintenanceRequests)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Maintenance_Rooms");
        });

        modelBuilder.Entity<MeterReading>(entity =>
        {
            entity.HasIndex(e => new { e.RoomId, e.ServiceId, e.BillingMonth }, "UQ_MeterReadings_Room_Service_Month").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CurrentReading).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.PreviousReading).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.RecordedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Room).WithMany(p => p.MeterReadings)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MeterReadings_Rooms");

            entity.HasOne(d => d.Service).WithMany(p => p.MeterReadings)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MeterReadings_Services");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasIndex(e => new { e.UserId, e.IsRead, e.CreatedAt }, "IX_Notifications_User_Read_Date").IsDescending(false, false, true);

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Content).HasMaxLength(2000);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Type)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("General");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notifications_Users");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasIndex(e => new { e.InvoiceId, e.Status }, "IX_Payments_Invoice_Status");

            entity.HasIndex(e => e.TransactionCode, "UX_Payments_TransactionCode")
                .IsUnique()
                .HasFilter("([TransactionCode] IS NOT NULL)");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.PaidAt).HasPrecision(0);
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TransactionCode)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Invoice).WithMany(p => p.Payments)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payments_Invoices");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasIndex(e => e.TokenHash, "UQ_RefreshTokens_TokenHash").IsUnique();

            entity.HasIndex(e => new { e.UserId, e.ExpiresAt }, "IX_RefreshTokens_User_ExpiresAt");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.TokenHash)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.ExpiresAt).HasPrecision(0);
            entity.Property(e => e.RevokedAt).HasPrecision(0);

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_RefreshTokens_Users");
        });

        modelBuilder.Entity<Resident>(entity =>
        {
            entity.HasIndex(e => e.UserId, "UQ_Residents_UserId").IsUnique();

            entity.HasIndex(e => e.IdentityNumber, "UX_Residents_IdentityNumber")
                .IsUnique()
                .HasFilter("([IdentityNumber] IS NOT NULL)");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.EmergencyContactName).HasMaxLength(150);
            entity.Property(e => e.EmergencyContactPhone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.IdentityNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PermanentAddress).HasMaxLength(500);

            entity.HasOne(d => d.User).WithOne(p => p.Resident)
                .HasForeignKey<Resident>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Residents_Users");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasIndex(e => new { e.FloorId, e.RoomNumber }, "UQ_Rooms_Floor_Number").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.Area).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.BasePrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.MaxOccupants).HasDefaultValue(1);
            entity.Property(e => e.RoomNumber).HasMaxLength(30);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Available");

            entity.HasOne(d => d.Floor).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.FloorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Rooms_Floors");
        });

        modelBuilder.Entity<RoomImage>(entity =>
        {
            entity.HasIndex(e => new { e.RoomId, e.SortOrder }, "IX_RoomImages_Room_Sort");

            entity.HasIndex(e => e.RoomId, "UX_RoomImages_OneThumbnailPerRoom")
                .IsUnique()
                .HasFilter("([IsThumbnail]=(1))");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.ImageUrl).HasMaxLength(1000);

            entity.HasOne(d => d.Room).WithOne(p => p.RoomImage)
                .HasForeignKey<RoomImage>(d => d.RoomId)
                .HasConstraintName("FK_RoomImages_Rooms");
        });

        modelBuilder.Entity<RoomService>(entity =>
        {
            entity.HasIndex(e => new { e.RoomId, e.ServiceId }, "UX_RoomServices_Active")
                .IsUnique()
                .HasFilter("([IsActive]=(1))");

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.DefaultQuantity)
                .HasDefaultValue(1m)
                .HasColumnType("decimal(12, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Room).WithMany(p => p.RoomServices)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoomServices_Rooms");

            entity.HasOne(d => d.Service).WithMany(p => p.RoomServices)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoomServices_Services");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasIndex(e => e.Name, "UQ_Services_Name").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CalculationType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.DefaultUnitPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Unit).HasMaxLength(30);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email, "UQ_Users_Email").IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newsequentialid())");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Resident");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
