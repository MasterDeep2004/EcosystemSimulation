using EcosystemSimulation.Data;
using EcosystemSimulation.Interfaces;
using EcosystemSimulation.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ==================================================
// Database
// ==================================================

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    );
});

// ==================================================
// Dependency Injection
// ==================================================

builder.Services.AddSingleton<ISimulationService, SimulationService>();

builder.Services.AddHostedService<SimulationRunner>();

// ==================================================
// MVC
// ==================================================

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// ==================================================
// CORS
// ==================================================

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ==================================================
// Build
// ==================================================

var app = builder.Build();

// ==================================================
// Middleware
// ==================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "Ecosystem Simulation API v1"
        );
    });
}

app.UseStaticFiles();

app.UseRouting();

app.UseCors();

app.MapControllers();

app.MapFallbackToFile("index.html");

// ==================================================
// Run
// ==================================================

app.Run();
