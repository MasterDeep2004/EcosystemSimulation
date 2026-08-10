using EcosystemSimulation.Data;
using EcosystemSimulation.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EcosystemSimulation.Controllers;

[ApiController]
[Route("api/simulation")]
public class SimulationController : ControllerBase
{
    private readonly ISimulationService _simulation;
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private readonly ILogger<SimulationController> _logger;

    public SimulationController(
        ISimulationService simulation,
        IDbContextFactory<AppDbContext> dbContextFactory,
        ILogger<SimulationController> logger)
    {
        _simulation = simulation;
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }


    // ==================================================
    // Real-time SSE Stream
    // ==================================================

    [HttpGet("stream")]
    public async Task Stream()
    {
        Response.ContentType = "text/event-stream";

        Response.Headers.CacheControl =
            "no-cache";

        Response.Headers["X-Accel-Buffering"] =
            "no";

        var token =
            HttpContext.RequestAborted;

        try
        {
            while (!token.IsCancellationRequested)
            {
                var state =
                    _simulation.GetBestState();

                if (state != null)
                {
                    var json =
                        JsonSerializer.Serialize(state);

                    await Response.WriteAsync(
                        $"data:{json}\n\n",
                        token);

                    await Response.Body.FlushAsync(
                        token);
                }

                await Task.Delay(
                    1000,
                    token);
            }
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation(
                "SSE client disconnected.");
        }
    }


    // ==================================================
    // Current Best State
    // ==================================================

    [HttpGet("best")]
    public IActionResult GetBestState()
    {
        var state =
            _simulation.GetBestState();

        if (state == null)
        {
            return NotFound(
                "Simulation has not started.");
        }

        return Ok(state);
    }


    // ==================================================
    // Current Population
    // ==================================================

    [HttpGet("population")]
    public IActionResult GetPopulation()
    {
        return Ok(
            _simulation.GetPopulation());
    }


    // ==================================================
    // Statistics
    // ==================================================

    [HttpGet("statistics")]
    public IActionResult GetStatistics()
    {
        return Ok(
            _simulation.GetStatistics());
    }


    // ==================================================
    // Database History
    // ==================================================

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory(
        [FromQuery] int count = 20)
    {
        count = Math.Clamp(count, 1, 100);

        await using var db =
            await _dbContextFactory
                .CreateDbContextAsync();

        var history =
            await db.SimulationStates
                .AsNoTracking()
                .OrderByDescending(x => x.Generation)
                .Take(count)
                .ToListAsync();

        return Ok(history);
    }


    // ==================================================
    // Restart
    // ==================================================

    [HttpPost("restart")]
    public IActionResult Restart()
    {
        _simulation.StartNewSimulation();

        _logger.LogInformation(
            "Simulation restarted.");

        return Ok(new
        {
            Message =
                "Simulation restarted successfully."
        });
    }


    // ==================================================
    // Health
    // ==================================================

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            Status = "Healthy",

            Generation =
                _simulation.Generation,

            Timestamp =
                DateTime.UtcNow
        });
    }
}
