using EmployeeMicroservice.Models;
using EmployeeMicroservice.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Serilog; // Add this using for logging

namespace EmployeeMicroservice.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                try
                {
                    Log.Information("Starting database seed...");

                    // Check if database has any employees
                    if (await context.Employees.AnyAsync())
                    {
                        Log.Information("Database already has employees, skipping seed");
                        return; // Database has been seeded
                    }

                    Log.Information("Seeding employees...");

                    var employees = new Employee[]
                    {
                        new Employee
                        {
                            Id = Guid.NewGuid(),
                            FirstName = "John",
                            LastName = "Doe",
                            Email = "john.doe@company.com",
                            DateOfBirth = new DateTime(1990, 5, 15),
                            Position = "Software Engineer",
                            Salary = 75000,
                            DepartmentId = Guid.NewGuid(),
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new Employee
                        {
                            Id = Guid.NewGuid(),
                            FirstName = "Jane",
                            LastName = "Smith",
                            Email = "jane.smith@company.com",
                            DateOfBirth = new DateTime(1985, 8, 22),
                            Position = "Product Manager",
                            Salary = 90000,
                            DepartmentId = Guid.NewGuid(),
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        },
                        new Employee
                        {
                            Id = Guid.NewGuid(),
                            FirstName = "Bob",
                            LastName = "Johnson",
                            Email = "bob.johnson@company.com",
                            DateOfBirth = new DateTime(1992, 3, 10),
                            Position = "DevOps Engineer",
                            Salary = 85000,
                            DepartmentId = Guid.NewGuid(),
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        }
                    };

                    await context.Employees.AddRangeAsync(employees);
                    await context.SaveChangesAsync();

                    Log.Information($"Seeded {employees.Length} employees");

                    // Check if skills already exist
                    if (!await context.Skills.AnyAsync())
                    {
                        Log.Information("Seeding skills...");

                        var skills = new Skill[]
                        {
                            new Skill
                            {
                                Id = Guid.NewGuid(),
                                Name = "C#",
                                Category = "Programming Languages"
                            },
                            new Skill
                            {
                                Id = Guid.NewGuid(),
                                Name = "ASP.NET Core",
                                Category = "Frameworks"
                            },
                            new Skill
                            {
                                Id = Guid.NewGuid(),
                                Name = "Entity Framework Core",
                                Category = "Frameworks"
                            },
                            new Skill
                            {
                                Id = Guid.NewGuid(),
                                Name = "SQL Server",
                                Category = "Databases"
                            },
                            new Skill
                            {
                                Id = Guid.NewGuid(),
                                Name = "Azure",
                                Category = "Cloud"
                            },
                            new Skill
                            {
                                Id = Guid.NewGuid(),
                                Name = "Docker",
                                Category = "DevOps"
                            },
                            new Skill
                            {
                                Id = Guid.NewGuid(),
                                Name = "Kubernetes",
                                Category = "DevOps"
                            },
                            new Skill
                            {
                                Id = Guid.NewGuid(),
                                Name = "JavaScript",
                                Category = "Programming Languages"
                            },
                            new Skill
                            {
                                Id = Guid.NewGuid(),
                                Name = "React",
                                Category = "Frameworks"
                            },
                            new Skill
                            {
                                Id = Guid.NewGuid(),
                                Name = "Python",
                                Category = "Programming Languages"
                            }
                        };

                        await context.Skills.AddRangeAsync(skills);
                        await context.SaveChangesAsync();

                        Log.Information($"Seeded {skills.Length} skills");
                    }

                    Log.Information("Database seed completed successfully");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error occurred while seeding database");
                    throw;
                }
            }
        }
    }
}