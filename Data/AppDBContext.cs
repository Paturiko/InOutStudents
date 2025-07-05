using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TimeLog> TimeLogs { get; set; }
    public DbSet<ZkUsers> ZkUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // === ZkUsers setup ===
        modelBuilder.Entity<ZkUsers>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<ZkUsers>()
            .HasIndex(u => u.AccessNumber)
            .IsUnique();

        // === TimeLog setup ===
        modelBuilder.Entity<TimeLog>()
            .HasKey(t => t.Id);

        modelBuilder.Entity<TimeLog>()
            .Property(t => t.TimeLogStamp)
            .IsRequired();

        modelBuilder.Entity<TimeLog>()
            .Property(t => t.DateCreated)
            .HasDefaultValueSql("SYSDATETIMEOFFSET()");

        modelBuilder.Entity<TimeLog>()
            .Property(t => t.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false)
            .ValueGeneratedNever();

        // Optional strings - don't mark as IsRequired, since DB allows NULL
        modelBuilder.Entity<TimeLog>()
            .Property(t => t.LogType)
            .HasMaxLength(10);

        modelBuilder.Entity<TimeLog>()
            .Property(t => t.DeviceSerialNumber)
            .HasDefaultValue("JHT4243000082");

        modelBuilder.Entity<TimeLog>()
            .Property(t => t.VerifyMode)
            .HasDefaultValue("4");

        modelBuilder.Entity<TimeLog>()
            .Property(t => t.Location)
            .HasDefaultValue(string.Empty);

        modelBuilder.Entity<TimeLog>()
            .Property(t => t.Checksum)
            .HasDefaultValue(string.Empty);

        // === Relationships ===
        modelBuilder.Entity<TimeLog>()
            .HasOne(t => t.ZkUsers)
            .WithMany(z => z.TimeLogs)
            .HasPrincipalKey(z => z.AccessNumber)
            .HasForeignKey(t => t.AccessNumber)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
