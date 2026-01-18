using Microsoft.EntityFrameworkCore;
using secure_code.Models;

namespace secure_code.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity to match Oracle database schema
        modelBuilder.Entity<User>(entity =>
        {
            // Try different table name configurations for case sensitivity
            entity.ToTable("USERS");  // Default uppercase

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("ID").ValueGeneratedNever();
            entity.Property(e => e.FirstName).HasColumnName("FIRSTNAME").IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).HasColumnName("LASTNAME").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Password).HasColumnName("PASSWORD").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Salary).HasColumnName("SALARY");
            entity.Property(e => e.Money).HasColumnName("MONEY");
            entity.Property(e => e.JsonData).HasColumnName("JSON_DATA").IsRequired().HasMaxLength(4000);

            // Log the table mapping for debugging
            Console.WriteLine("Entity Framework User entity mapped to table: USERS");
        });
    }
}