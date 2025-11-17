using EcosystemSimulation.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EcosystemSimulation.Services
{
    public class SimulationRunner : BackgroundService
    {
        private readonly SimulationService _simulation;
        private readonly int _delayMs = 2000;

        public SimulationRunner(SimulationService simulation)
        {
            _simulation = simulation ?? throw new ArgumentNullException(nameof(simulation));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("[SimulationRunner] Background simulation started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _simulation.RunGeneration();
                    var best = _simulation.GetBestState();
                    if (best != null)
                    {
                        Console.WriteLine($"🌱{best.Plants} 🐇{best.Herbivores} 🦁{best.Carnivores} | Fitness={best.Fitness():F2}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SimulationRunner] Error: {ex.Message}");
                }

                try
                {
                    await Task.Delay(_delayMs, stoppingToken);
                }
                catch (TaskCanceledException) { break; }
            }

            Console.WriteLine("[SimulationRunner] Simulation stopped.");
        }
    }
}
