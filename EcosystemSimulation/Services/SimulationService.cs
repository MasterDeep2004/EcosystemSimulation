using EcosystemSimulation.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EcosystemSimulation.Services
{
    /// <summary>
    /// Runs the ecosystem simulation using
    /// Genetic Algorithm + Lotka Volterra population model.
    /// </summary>
    public class SimulationService : ISimulationService
    {
        private int _generationCounter = 0;

        private readonly int _populationSize;

        private const int TournamentSize = 3;

        private const double MutationRate = 0.25;

        private readonly List<SimulationState> _population = new();

        private readonly object _lock = new();

        private readonly ILogger<SimulationService> _logger;

        private static readonly Random _rand = Random.Shared;

        public int Generation => _generationCounter;

        public SimulationService(
            IConfiguration configuration,
            ILogger<SimulationService> logger)
        {
            _logger = logger;

            _populationSize =
                configuration.GetValue<int>(
                    "Simulation:PopulationSize",
                    20);

            InitializePopulation();
        }

        /// <summary>
        /// Creates the initial ecosystem population.
        /// </summary>
        private void InitializePopulation()
        {
            lock (_lock)
            {
                _population.Clear();

                for (int i = 0; i < _populationSize; i++)
                {
                    _population.Add(new SimulationState
                    {
                        Id = i + 1,
                        Plants = _rand.Next(800, 1200),
                        Herbivores = _rand.Next(200, 400),
                        Carnivores = _rand.Next(30, 80),
                        Timestamp = DateTime.UtcNow
                    });
                }
            }

            _logger.LogInformation(
                "Initial population created with {Count} states.",
                _populationSize);
        }

        /// <summary>
        /// Calculates fitness of one ecosystem state.
        /// </summary>
        public double Fitness(SimulationState state)
        {
            if (state == null)
                return 0;

            try
            {
                double balanceScore =
                    1.0 /
                    (
                        1 +
                        Math.Abs(state.Plants - state.Herbivores) +
                        Math.Abs(state.Herbivores - state.Carnivores)
                    );

                double diversityScore =
                    state.Plants > 200 &&
                    state.Herbivores > 50 &&
                    state.Carnivores > 10
                        ? 1.2
                        : 0.8;

                double totalPopulation =
                    state.Plants +
                    state.Herbivores +
                    state.Carnivores;

                return totalPopulation *
                       balanceScore *
                       diversityScore;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Fitness calculation failed.");

                return 0;
            }
        }

        /// <summary>
        /// Tournament Selection.
        /// </summary>
        private SimulationState SelectParent()
        {
            lock (_lock)
            {
                var candidates = new List<SimulationState>();

                for (int i = 0; i < TournamentSize; i++)
                {
                    candidates.Add(
                        _population[
                            _rand.Next(_population.Count)
                        ]);
                }

                return candidates
                    .OrderByDescending(Fitness)
                    .First()
                    .Clone();
            }
        }
        /// <summary>
        /// Creates a child by combining two parents.
        /// </summary>
        private SimulationState Crossover(
            SimulationState parent1,
            SimulationState parent2)
        {
            if (parent1 == null || parent2 == null)
            {
                return new SimulationState
                {
                    Id = (_generationCounter * 1000) + _rand.Next(1000),
                    Plants = 100,
                    Herbivores = 50,
                    Carnivores = 5,
                    Timestamp = DateTime.UtcNow
                };
            }

            return new SimulationState
            {
                Id = (_generationCounter * 1000) + _rand.Next(1000),

                Plants = Math.Max(
                    100,
                    (parent1.Plants + parent2.Plants) / 2 +
                    _rand.Next(-30, 31)
                ),

                Herbivores = Math.Max(
                    50,
                    (parent1.Herbivores + parent2.Herbivores) / 2 +
                    _rand.Next(-20, 21)
                ),

                Carnivores = Math.Max(
                    5,
                    (parent1.Carnivores + parent2.Carnivores) / 2 +
                    _rand.Next(-10, 11)
                ),

                Timestamp = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Applies mutation and ecosystem dynamics.
        /// </summary>
        private void Mutate(SimulationState state)
        {
            if (state == null)
                return;

            try
            {
                // ---------- Random Mutation ----------

                if (_rand.NextDouble() < MutationRate)
                    state.Plants += _rand.Next(-50, 51);

                if (_rand.NextDouble() < MutationRate)
                    state.Herbivores += _rand.Next(-25, 26);

                if (_rand.NextDouble() < MutationRate)
                    state.Carnivores += _rand.Next(-10, 11);

                // ---------- Lotka-Volterra Dynamics ----------

                int plantGrowth =
                    (int)(
                        0.15 * state.Plants -
                        0.002 * state.Plants * state.Herbivores
                    );

                state.Plants += plantGrowth;

                int herbivoreChange =
                    (int)(
                        0.003 * state.Plants * state.Herbivores -
                        0.08 * state.Herbivores -
                        0.002 * state.Herbivores * state.Carnivores
                    );

                state.Herbivores += herbivoreChange;

                int carnivoreChange =
                    (int)(
                        0.0015 * state.Herbivores * state.Carnivores -
                        0.06 * state.Carnivores
                    );

                state.Carnivores += carnivoreChange;

                // ---------- Environmental Events ----------

                if (_rand.NextDouble() < 0.05)
                {
                    _logger.LogInformation(
                        "Environmental Event: Drought");

                    state.Plants =
                        (int)(state.Plants * 0.70);
                }

                if (_rand.NextDouble() < 0.05)
                {
                    _logger.LogInformation(
                        "Environmental Event: Herbivore Boom");

                    state.Herbivores +=
                        _rand.Next(30, 70);
                }

                // ---------- Population Constraints ----------

                state.Plants =
                    Math.Max(100, state.Plants);

                state.Herbivores =
                    Math.Max(50, state.Herbivores);

                state.Carnivores =
                    Math.Max(5, state.Carnivores);

                // Predator population cannot exceed 40% of herbivores
                state.Carnivores =
                    Math.Min(
                        state.Carnivores,
                        (int)(0.4 * state.Herbivores));

                state.Timestamp = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Mutation failed.");
            }
        }

               /// <summary>
        /// Executes one complete generation of the Genetic Algorithm.
        /// </summary>
        public void RunGeneration()
        {
            try
            {
                var newPopulation = new List<SimulationState>();

                for (int i = 0; i < _populationSize; i++)
                {
                    var parent1 = SelectParent();
                    var parent2 = SelectParent();

                    var child = Crossover(parent1, parent2);

                    Mutate(child);

                    newPopulation.Add(child);
                }

                lock (_lock)
                {
                    _population.Clear();
                    _population.AddRange(newPopulation);
                }

                _generationCounter++;

                _logger.LogInformation(
                    "Generation {Generation} completed successfully.",
                    _generationCounter);

                PrintPopulation();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while running simulation generation.");
            }
        }

        /// <summary>
        /// Prints the current population to the application log.
        /// </summary>
        private void PrintPopulation()
        {
            lock (_lock)
            {
                foreach (var state in _population)
                {
                    _logger.LogDebug(
                        "Plants:{Plants} Herbivores:{Herbivores} Carnivores:{Carnivores} Fitness:{Fitness:F2}",
                        state.Plants,
                        state.Herbivores,
                        state.Carnivores,
                        Fitness(state));
                }
            }
        }

        /// <summary>
        /// Resets the simulation.
        /// </summary>
        public void StartNewSimulation()
        {
            _generationCounter = 0;

            InitializePopulation();

            _logger.LogInformation("Simulation restarted.");
        }

        /// <summary>
        /// Returns the best ecosystem state.
        /// </summary>
        public SimulationState? GetBestState()
        {
            try
            {
                lock (_lock)
                {
                    return _population
                        .OrderByDescending(Fitness)
                        .FirstOrDefault()
                        ?.Clone();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unable to retrieve best simulation state.");

                return null;
            }
        }

        /// <summary>
        /// Returns a snapshot of the current population.
        /// </summary>
        public IEnumerable<SimulationState> GetPopulation()
        {
            lock (_lock)
            {
                return _population
                    .Select(state => state.Clone())
                    .ToList();
            }
        }
    }
}
