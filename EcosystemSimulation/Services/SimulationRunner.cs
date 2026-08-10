using EcosystemSimulation.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EcosystemSimulation.Services
{
    public class SimulationRunner : BackgroundService
    {
        private readonly ISimulationService _simulation;
        private readonly ILogger<SimulationRunner> _logger;
        private readonly int _delayMs;

        public SimulationRunner(
            ISimulationService simulation,
            IConfiguration configuration,
            ILogger<SimulationRunner> logger)
        {
            _simulation =
                simulation ??
                throw new ArgumentNullException(
                    nameof(simulation));

            _logger =
                logger ??
                throw new ArgumentNullException(
                    nameof(logger));

            _delayMs =
                configuration.GetValue<int>(
                    "Simulation:SimulationDelay",
                    1000);
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Simulation Runner started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _simulation.RunGeneration();

                    var best =
                        _simulation.GetBestState();

                    if (best != null)
                    {
                        _logger.LogInformation(
                            "Generation {Generation} | Plants={Plants} | Herbivores={Herbivores} | Carnivores={Carnivores} | Fitness={Fitness:F2} | Event={Event}",
                            _simulation.Generation,
                            best.Plants,
                            best.Herbivores,
                            best.Carnivores,
                            _simulation.Fitness(best),
                            string.IsNullOrWhiteSpace(
                                best.EventName)
                                ? "None"
                                : best.EventName);
                    }

                    await Task.Delay(
                        _delayMs,
                        stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation(
                        "Simulation Runner cancelled.");

                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Unexpected error while running simulation.");
                }
            }

            _logger.LogInformation(
                "Simulation Runner stopped.");
        }
    }
}
