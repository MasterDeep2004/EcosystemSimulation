using EcosystemSimulation.Services;
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
        SimulationService simulation,
        ILogger<SimulationController> logger)
    {
        _simulation = simulation;
        _logger = logger;
    }

    [HttpGet("stream")]
    public async Task Stream()
    {
        Response.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";

        var token = HttpContext.RequestAborted;

        while (!token.IsCancellationRequested)
        {
            var state = _simulation.GetBestState();

            if (state != null)
            {
                await Response.WriteAsync(
                    $"data:{JsonSerializer.Serialize(state)}\n\n",
                    token);

                await Response.Body.FlushAsync(token);
            }

            await Task.Delay(1000, token);
        }

        _logger.LogInformation("SSE client disconnected.");
    }

    [HttpGet("best")]
    public IActionResult GetBestState()
    {
        return Ok(_simulation.GetBestState());
    }

    [HttpGet("population")]
    public IActionResult GetPopulation()
    {
        return Ok(_simulation.GetPopulation());
    }

    [HttpPost("restart")]
    public IActionResult Restart()
    {
        _simulation.StartNewSimulation();

        return Ok(new
        {
            Message = "Simulation restarted successfully."
        });
    }
}
