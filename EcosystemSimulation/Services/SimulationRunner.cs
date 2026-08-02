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
            _simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Simulation Runner started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _simulation.RunGeneration();

                    var best = _simulation.GetBestState();

                    if (best != null)
                    {
                        _logger.LogInformation(
                            "Generation completed | Plants: {Plants}, Herbivores: {Herbivores}, Carnivores: {Carnivores}, Fitness: {Fitness:F2}",
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
                    _logger.LogError(ex, "An error occurred while running the simulation.");
                }
            }

            _logger.LogInformation("Simulation Runner stopped.");
        }
    }
}
