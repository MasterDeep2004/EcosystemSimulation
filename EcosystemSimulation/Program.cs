using EcosystemSimulation.Data;
using EcosystemSimulation.Interfaces;
using EcosystemSimulation.Services;
using Microsoft.EntityFrameworkCore;
using OpenAI.Responses;

var builder = WebApplication.CreateBuilder(args);

// ==================================================
// Configuration
// ==================================================

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "DefaultConnection was not found.");
}

// ==================================================
// Database
// ==================================================

builder.Services.AddDbContextFactory<AppDbContext>(options =>
{
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString));
});

// ==================================================
// Repository
// ==================================================

builder.Services.AddScoped<ISimulationRepository,
    SimulationRepository>();

// ==================================================
// Simulation
// ==================================================

builder.Services.AddSingleton<ISimulationService,
    SimulationService>();

builder.Services.AddHostedService<SimulationRunner>();

// ==================================================
// AI
// ==================================================

var openAiKey =
    builder.Configuration["OpenAI:ApiKey"];

if (!string.IsNullOrWhiteSpace(openAiKey))
{
    builder.Services.AddSingleton(
        new ResponsesClient(openAiKey));

    builder.Services.AddScoped<IAIAnalysisService,
        AIAnalysisService>();
}

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
// Swagger
// ==================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "Ecosystem Simulation API v1");
    });
}

// ==================================================
// Middleware
// ==================================================

app.UseStaticFiles();

app.UseRouting();

app.UseCors();

app.MapControllers();

app.MapFallbackToFile("index.html");

// ==================================================
// Run
// ==================================================

app.Run();
