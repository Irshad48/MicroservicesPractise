using microservice1.Infrastructure.Resilience;
using microservice1.Services;
using microservice1.Services.Interfaces;
using Polly;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
// Register HttpClientFactory
/*builder.Services.AddHttpClient("Service2Client", client =>
{
    client.BaseAddress =  new Uri(builder.Configuration["Services:Service2BaseUrl"] ?? throw new InvalidOperationException("Service2BaseUrl is not configured"));
    // Set default headers if needed - below is an example of setting Accept header to application/json
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    // Set timeout as neede. its essential to avoid hanging requests because one slow service can block others
    client.Timeout = TimeSpan.FromSeconds(30);
});*/

builder.Services.AddHttpClient<IService2Client, Service2Client>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:Service2BaseUrl"] ?? throw new InvalidOperationException("Service2BaseUrl is not configured"));
    // Set timeout as neede. its essential to avoid hanging requests because one slow service can block others
    client.Timeout = TimeSpan.FromSeconds(3);
})
.AddPolicyHandler(PollyPolicies.GetResiliencePolicy());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); 
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Microservice1 API V1"); });;
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
