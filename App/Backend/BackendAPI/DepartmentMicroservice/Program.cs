using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using DepartmentMicroservice.Data;
using DepartmentMicroservice.Services.Repository;
using DepartmentMicroservice.Helpers;
using DepartmentMicroservice.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/departmentms-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

// API versioning explorer (used by OpenAPI/Scalar)
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Register DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories and UnitOfWork
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(DepartmentMicroservice.Helpers.MappingProfiles).Assembly);

// Add OpenAPI document source used by Scalar (replaces direct Swagger UI in this service)
builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new()
        {
            Title = "Department Microservice API",
            Version = "v1",
            Description = "Department microservice OpenAPI document",
            Contact = new()
            {
                Name = "Development Team",
                Email = "dev@company.com"
            },
            License = new()
            {
                Name = "Private - Internal Use Only"
            }
        };
        return Task.CompletedTask;
    });
});

var app = builder.Build();

// Development-only: map OpenAPI endpoint and Scalar UI (no Swagger UI)
if (app.Environment.IsDevelopment())
{
    // Expose OpenAPI JSON endpoint for Scalar/OpenAPI consumers
    app.MapOpenApi();

    // Expose Scalar API reference UI
    app.MapScalarApiReference(options =>
    {
        options.Title = "Department Microservice API";
        options.Theme = ScalarTheme.Purple;
        options.DefaultHttpClient = new KeyValuePair<ScalarTarget, ScalarClient>(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });

    // Ensure database exists (create if missing) in development
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            Log.Information("Ensuring database is created...");
            context.Database.EnsureCreated();
            Log.Information("Database ensured.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while ensuring the database.");
        }
    }
}
else
{
    app.UseHsts();
}

app.UseSerilogRequestLogging();

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Starting DepartmentMicroservice");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
