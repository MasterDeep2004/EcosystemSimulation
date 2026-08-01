using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EcosystemSimulation.Services
{
    public class SimulationRunner : BackgroundService
    {
        private readonly SimulationService _simulation;
        private readonly ILogger<SimulationRunner> _logger;
        private readonly IConfiguration _configuration;

        public SimulationRunner(
            SimulationService simulation,
            ILogger<SimulationRunner> logger,
            IConfiguration configuration)
        {
            _simulation = simulation;
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Simulation Background Service Started.");

            int delay =
                _configuration.GetValue<int>("Simulation:GenerationDelay", 2000);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _simulation.RunGeneration();

                    var best = _simulation.GetBestState();

                    if (best != null)
                    {
                        _logger.LogInformation(
                            "Generation {Generation} | Plants:{Plants} Herbivores:{Herbivores} Carnivores:{Carnivores} Fitness:{Fitness:F2}",
                            _simulation.Generation,
                            best.Plants,
                            best.Herbivores,
                            best.Carnivores,
                            best.Fitness());
                    }

                    await Task.Delay(delay, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Simulation Runner Error");
                }
            }

            _logger.LogInformation("Simulation Background Service Stopped.");
        }
    }
}
