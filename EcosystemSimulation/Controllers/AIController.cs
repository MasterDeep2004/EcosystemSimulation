using EcosystemSimulation.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcosystemSimulation.Controllers
{
    [ApiController]
    [Route("api/ai")]
    public class AIController : ControllerBase
    {
        private readonly ISimulationService _simulation;
        private readonly IAIAnalysisService _ai;

        public AIController(
            ISimulationService simulation,
            IAIAnalysisService ai)
        {
            _simulation = simulation;
            _ai = ai;
        }

        [HttpGet("analyze")]
        public async Task<IActionResult> Analyze(
            CancellationToken cancellationToken)
        {
            var state = _simulation.GetBestState();

            if (state == null)
            {
                return NotFound(
                    "No simulation state is available.");
            }

            var result =
                await _ai.AnalyzeAsync(
                    state,
                    cancellationToken);

            return Ok(result);
        }
    }
}
