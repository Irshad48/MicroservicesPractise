using DepartmentMicroservice.Models;
using Microsoft.EntityFrameworkCore;

namespace DepartmentMicroservice.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Department>(b =>
            {
                b.HasKey(d => d.Id);
                b.Property(d => d.Id).HasDefaultValueSql("NEWID()");
                b.Property(d => d.Name).IsRequired().HasMaxLength(200);
                b.Property(d => d.Description).HasMaxLength(1000).IsRequired(false);
                b.Property(d => d.IsActive).HasDefaultValue(true);
                b.Property(d => d.CreatedAt).HasDefaultValueSql("GETDATE()");
                b.Property(d => d.UpdatedAt).IsRequired(false);
                b.HasIndex(d => d.Name).IsUnique(false);
            });
        }
    }
}