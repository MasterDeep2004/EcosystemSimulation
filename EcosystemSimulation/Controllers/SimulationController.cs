using EcosystemSimulation.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EcosystemSimulation.Controllers;

[ApiController]
[Route("api/simulation")]
public class SimulationController : ControllerBase
{
    private readonly ISimulationService _simulation;
    private readonly ILogger<SimulationController> _logger;

    public SimulationController(
        ISimulationService simulation,
        ILogger<SimulationController> logger)
    {
        _simulation = simulation;
        _logger = logger;
    }

    /// <summary>
    /// Streams the best simulation state using Server-Sent Events (SSE).
    /// </summary>
    [HttpGet("stream")]
    public async Task Stream()
    {
        Response.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";

        var token = HttpContext.RequestAborted;

        try
        {
            while (!token.IsCancellationRequested)
            {
                var state = _simulation.GetBestState();

                if (state != null)
                {
                    var json = JsonSerializer.Serialize(state);

                    await Response.WriteAsync($"data:{json}\n\n", token);

                    await Response.Body.FlushAsync(token);
                }

                await Task.Delay(1000, token);
            }
        }
        catch (TaskCanceledException)
        {
            _logger.LogInformation("SSE client disconnected.");
        }
    }

    /// <summary>
    /// Returns the best ecosystem state.
    /// </summary>
    [HttpGet("best")]
    public IActionResult GetBestState()
    {
        var state = _simulation.GetBestState();

        if (state == null)
            return NotFound("Simulation has not started.");

        return Ok(state);
    }

    /// <summary>
    /// Returns the current simulation population.
    /// </summary>
    [HttpGet("population")]
    public IActionResult GetPopulation()
    {
        return Ok(_simulation.GetPopulation());
    }

    /// <summary>
    /// Returns simulation statistics.
    /// </summary>
    [HttpGet("stats")]
    public IActionResult GetStatistics()
    {
        return Ok(_simulation.GetStatistics());
    }

    /// <summary>
    /// Restarts the simulation.
    /// </summary>
    [HttpPost("restart")]
    public IActionResult Restart()
    {
        _simulation.StartNewSimulation();

        _logger.LogInformation("Simulation restarted.");

        return Ok(new
        {
            Message = "Simulation restarted successfully."
        });
    }

    /// <summary>
    /// Health endpoint.
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            Status = "Healthy",
            Generation = _simulation.Generation,
            Timestamp = DateTime.UtcNow
        });
    }
}
