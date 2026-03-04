using EmployeeMicroservice.Data;
using EmployeeMicroservice.Helpers;
using EmployeeMicroservice.Interfaces;
using EmployeeMicroservice.Services;
using EmployeeMicroservice.Services.External;
using EmployeeMicroservice.Services.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/employeems-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();

// Add API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

// Add API Versioning Explorer
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Add DbContext with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfiles));

// Register Unit of Work and Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
// Add OpenAPI/Swagger document generation
builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new()
        {
            Title = "Employee Microservice API",
            Version = "v1",
            Description = "A modern employee management microservice with best practices",
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
builder.Services.AddHttpClient<IDepartmentServiceClient, DepartmentServiceClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ServiceUrls:DepartmentService"]);
})
.SetHandlerLifetime(TimeSpan.FromMinutes(5));

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Map OpenAPI endpoint
    app.MapOpenApi();

    // Map Scalar UI for API documentation - Corrected configuration
    app.MapScalarApiReference(options =>
    {
        // Basic configuration that works with current Scalar version
        options.Title = "Employee Microservice API";
        options.Theme = ScalarTheme.Purple;
        options.DefaultHttpClient = new KeyValuePair<ScalarTarget, ScalarClient>(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });

    // FIXED: Create database and tables, THEN seed
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();

            // IMPORTANT: This creates the database and tables if they don't exist
            Log.Information("Ensuring database is created...");
            await context.Database.EnsureCreatedAsync();

            // Now seed the data
            Log.Information("Seeding database...");
            await SeedData.Initialize(services);

            Log.Information("Database setup completed successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred setting up the database.");
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
    Log.Information("Starting EmployeeMicroservice with OpenAPI and Scalar UI");
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