using EmployeeMicroservice.Models;
using Microsoft.EntityFrameworkCore;
namespace EmployeeMicroservice.Data
{
    public class ApplicationDbContext : DbContext
    {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
            {
            }
            public DbSet<Employee> Employees { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                modelBuilder.Entity<Employee>(entity =>
                {
                    entity.HasKey(e => e.Id);
                    entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                    entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                    entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                    entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                    entity.HasIndex(e => e.Email).IsUnique();
                    entity.Property(e => e.DateOfBirth).IsRequired();
                    entity.Property(e => e.Position).IsRequired().HasMaxLength(100);
                    entity.Property(e => e.Salary).HasColumnType("decimal(18,2)");
                    entity.Property(e => e.IsActive).HasDefaultValue(true);
                    entity.Property(e => e.DepartmentId).IsRequired();
                    entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                    entity.Property(e => e.UpdatedAt).IsRequired(false);
                });
            
        }
    }
}
