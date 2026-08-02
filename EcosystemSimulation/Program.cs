using EcosystemSimulation.Data;
using EcosystemSimulation.Interfaces;
using EcosystemSimulation.Repositories;
using EcosystemSimulation.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ------------------- Database -------------------

builder.Services.AddDbContext<SimulationDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(
            builder.Configuration.GetConnectionString("DefaultConnection")));
});

// ------------------- Dependency Injection -------------------

builder.Services.AddSingleton<ISimulationService, SimulationService>();

builder.Services.AddScoped<ISimulationRepository, SimulationRepository>();

builder.Services.AddHostedService<SimulationRunner>();

// ------------------- MVC -------------------

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

// ------------------- Apply Database Migration -------------------

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SimulationDbContext>();

    db.Database.Migrate();
}

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

app.MapFallbackToFile("index.html");

// ------------------- Run -------------------

app.Run();
