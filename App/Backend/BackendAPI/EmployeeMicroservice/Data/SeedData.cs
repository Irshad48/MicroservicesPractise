using EmployeeMicroservice.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeMicroservice.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Check if database exists and has any employees
                if (context.Employees.Any())
                {
                    return; // Database has been seeded
                }

                var employees = new Employee[]
                {
                    new Employee
                    {
                        FirstName = "John",
                        LastName = "Doe",
                        Email = "john.doe@company.com",
                        DateOfBirth = new DateTime(1990, 5, 15),
                        Position = "Software Engineer",
                        Salary = 75000,
                        DepartmentId = Guid.NewGuid(),
                        IsActive = true
                    },
                    new Employee
                    {
                        FirstName = "Jane",
                        LastName = "Smith",
                        Email = "jane.smith@company.com",
                        DateOfBirth = new DateTime(1985, 8, 22),
                        Position = "Product Manager",
                        Salary = 90000,
                        DepartmentId = Guid.NewGuid(),
                        IsActive = true
                    },
                    new Employee
                    {
                        FirstName = "Bob",
                        LastName = "Johnson",
                        Email = "bob.johnson@company.com",
                        DateOfBirth = new DateTime(1992, 3, 10),
                        Position = "DevOps Engineer",
                        Salary = 85000,
                        DepartmentId = Guid.NewGuid(),
                        IsActive = true
                    }
                };

                await context.Employees.AddRangeAsync(employees);
                await context.SaveChangesAsync();
            }
        }
    }
}