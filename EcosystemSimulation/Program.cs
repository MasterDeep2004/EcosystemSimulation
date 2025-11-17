using EcosystemSimulation.Services;

var builder = WebApplication.CreateBuilder(args);

// ------------------- Services -------------------
builder.Services.AddSingleton<SimulationService>();
builder.Services.AddHostedService<SimulationRunner>();
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

// Enable Swagger only in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ecosystem Simulation API v1");
    });
}

// Serve static files from wwwroot
app.UseStaticFiles();

// Enable CORS
app.UseCors();

// Map API controllers
app.MapControllers();

// ------------------- Run -------------------
Console.WriteLine("Backend running at http://localhost:5172");
app.Urls.Add("http://localhost:5172");
app.Run();
