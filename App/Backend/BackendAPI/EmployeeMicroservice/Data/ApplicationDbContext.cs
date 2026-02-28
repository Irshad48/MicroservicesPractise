using EmployeeMicroservice.Models;
using EmployeeMicroservice.Models.Entities;
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
        public DbSet<Skill> Skills { get; set; }
        public DbSet<EmployeeSkill> EmployeeSkills { get; set; }

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
            
                modelBuilder.Entity<Skill>(entity =>
                {
                    entity.HasKey(s => s.Id);
                    entity.Property(s => s.Id).HasDefaultValueSql("NEWID()");
                    entity.Property(s => s.Name).IsRequired().HasMaxLength(100);
                    entity.HasIndex(s => s.Name).IsUnique().HasDatabaseName("IX_Skills_Name");
                    entity.Property(s => s.Category).HasMaxLength(50);
                });

                modelBuilder.Entity<EmployeeSkill>(entity =>
                {
                    entity.HasKey(es => new { es.EmployeeId, es.SkillId });
                    entity.HasOne(es => es.Employee)
                        .WithMany(e => e.EmployeeSkills)
                        .HasForeignKey(es => es.EmployeeId)
                        .OnDelete(DeleteBehavior.Cascade);

                    entity.HasOne(es => es.Skill)
                    .WithMany(s => s.EmployeeSkills)
                    .HasForeignKey(es => es.SkillId)
                    .OnDelete(DeleteBehavior.Restrict);

                    entity.Property(es => es.ProficiencyLevel)
                        .IsRequired()
                        .HasConversion<int>();

                    entity.Property(es => es.YearsOfExperience).IsRequired();
                    entity.Property(es => es.CreatedAt).HasDefaultValueSql("GETDATE()");

                });
        }   
    }
}
