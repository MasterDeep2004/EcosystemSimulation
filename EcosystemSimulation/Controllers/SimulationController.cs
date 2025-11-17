using Microsoft.AspNetCore.Mvc;
using EcosystemSimulation.Services;
using System.Text.Json;
using System.Threading.Tasks;

namespace EcosystemSimulation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimulationController : ControllerBase
    {
        private readonly SimulationService _sim;
        public SimulationController(SimulationService sim) => _sim = sim;
        [HttpGet("stream")]
        public async Task Stream()
        {
            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");

            var token = HttpContext.RequestAborted;

            while (!token.IsCancellationRequested)
            {
                var best = _sim.GetBestState();
                if (best == null) continue;

                var json = JsonSerializer.Serialize(best);

                // Critical: flush after every write
                await Response.WriteAsync($"data: {json}\n\n", token);
                await Response.Body.FlushAsync(token);

                await Task.Delay(1000, token);
            }
        }
    }
}

