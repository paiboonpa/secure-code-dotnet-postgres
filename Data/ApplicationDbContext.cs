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
            entity.ToTable("users");  // Default lowercase

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
            entity.Property(e => e.FirstName).HasColumnName("firstname").IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).HasColumnName("lastname").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Password).HasColumnName("password").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Salary).HasColumnName("salary");
            entity.Property(e => e.Money).HasColumnName("money");
            entity.Property(e => e.JsonData).HasColumnName("json_data").IsRequired().HasMaxLength(4000);
            // Log the table mapping for debugging
            Console.WriteLine("Entity Framework User entity mapped to table: users");
        });
    }
}