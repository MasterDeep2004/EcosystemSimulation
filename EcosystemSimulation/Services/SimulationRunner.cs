using EcosystemSimulation.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EcosystemSimulation.Services
{
    public class SimulationRunner : BackgroundService
    {
        private readonly SimulationService _simulation;
        private readonly ILogger<SimulationRunner> _logger;
        private readonly int _delayMs = 2000;

        public SimulationRunner(
            SimulationService simulation,
            ILogger<SimulationRunner> logger)
        {
            _simulation = simulation;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Simulation Runner Started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _simulation.RunGeneration();

                    var best = _simulation.GetBestState();

                    if (best != null)
                    {
                        _logger.LogInformation(
                            "Generation Completed | Plants:{Plants} Herbivores:{Herbivores} Carnivores:{Carnivores} Fitness:{Fitness:F2}",
                            best.Plants,
                            best.Herbivores,
                            best.Carnivores,
                            best.Fitness());
                    }

                    await Task.Delay(_delayMs, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("Simulation cancelled.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while running simulation.");
                }
            }

            _logger.LogInformation("Simulation Runner Stopped.");
        }
    }
}
