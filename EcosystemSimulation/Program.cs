using EcosystemSimulation.Services;
using EcosystemSimulation.Interfaces;
using EcosystemSimulation.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ------------------- Services -------------------

builder.Services.AddSingleton<ISimulationService, SimulationService>();

builder.Services.AddHostedService<SimulationRunner>();

builder.Services.AddDbContext<SimulationDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(
            builder.Configuration.GetConnectionString("DefaultConnection"))
    );
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// ------------------- CORS -------------------

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

// ------------------- Middleware -------------------

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

app.UseStaticFiles();

app.UseRouting();

app.UseCors();

app.MapControllers();

// Default page
app.MapFallbackToFile("index.html");

// ------------------- Run -------------------

app.Run();
