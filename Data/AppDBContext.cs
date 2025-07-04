using Microsoft.EntityFrameworkCore;
using Reporter.Models;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Reporting> Reporting { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Reporting>().Property(r => r.Category).HasColumnName("Category");
        modelBuilder.Entity<Reporting>().Property(r => r.TimeIn).HasColumnName("TimeIn");
        modelBuilder.Entity<Reporting>().Property(r => r.Status).HasColumnName("Status");
        modelBuilder.Entity<Reporting>().Property(r => r.IsIn).HasColumnName("IsIn");
    }
}
