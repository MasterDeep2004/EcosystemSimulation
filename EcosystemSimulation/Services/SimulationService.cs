using EcosystemSimulation.Interfaces;
using EcosystemSimulation.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace EcosystemSimulation.Services
{
    public class SimulationService : ISimulationService
    {
        private readonly ILogger<SimulationService> _logger;
        private readonly object _lock = new();
        private static readonly Random _rand = Random.Shared;

        private readonly List<SimulationState> _population = new();

        private readonly int _populationSize;
        private readonly int _tournamentSize;
        private readonly double _mutationRate;

        private int _generationCounter = 0;

        public int Generation => _generationCounter;

        private const double PlantGrowthRate = 0.15;
        private const double PlantConsumptionRate = 0.002;

        private const double HerbivoreBirthRate = 0.003;
        private const double HerbivoreDeathRate = 0.08;
        private const double HerbivorePredationRate = 0.002;

        private const double CarnivoreBirthRate = 0.0015;
        private const double CarnivoreDeathRate = 0.06;

        public SimulationService(
            IConfiguration configuration,
            ILogger<SimulationService> logger)
        {
            _logger = logger;

            _populationSize = configuration.GetValue<int>(
                "Simulation:PopulationSize",
                20);

            _tournamentSize = configuration.GetValue<int>(
                "Simulation:TournamentSize",
                3);

            _mutationRate = configuration.GetValue<double>(
                "Simulation:MutationRate",
                0.25);

            InitializePopulation();
        }

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
                        Generation = 0,
                        EventName = string.Empty,
                        Timestamp = DateTime.UtcNow
                    });
                }
            }

            _logger.LogInformation(
                "Initialized simulation with {PopulationSize} ecosystem states.",
                _populationSize);
        }

        public double Fitness(SimulationState state)
        {
            if (state == null)
                return 0;

            try
            {
                double balanceScore =
                    1.0 /
                    (1 +
                     Math.Abs(state.Plants - state.Herbivores) +
                     Math.Abs(state.Herbivores - state.Carnivores));

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
                _logger.LogError(ex, "Fitness calculation failed.");
                return 0;
            }
            private SimulationState SelectParent()
{
    lock (_lock)
    {
        var candidates = new List<SimulationState>();

        for (int i = 0; i < _tournamentSize; i++)
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
            Generation = _generationCounter + 1,
            EventName = string.Empty,
            Timestamp = DateTime.UtcNow
        };
    }

    return new SimulationState
    {
        Id = (_generationCounter * 1000) + _rand.Next(1000),

        Plants = Math.Max(
            100,
            (parent1.Plants + parent2.Plants) / 2 +
            _rand.Next(-30, 31)),

        Herbivores = Math.Max(
            50,
            (parent1.Herbivores + parent2.Herbivores) / 2 +
            _rand.Next(-20, 21)),

        Carnivores = Math.Max(
            5,
            (parent1.Carnivores + parent2.Carnivores) / 2 +
            _rand.Next(-10, 11)),

        Generation = _generationCounter + 1,

        EventName = string.Empty,

        Timestamp = DateTime.UtcNow
    };
}

private void Mutate(SimulationState state)
{
    if (state == null)
        return;

    try
    {
        if (_rand.NextDouble() < _mutationRate)
            state.Plants += _rand.Next(-50, 51);

        if (_rand.NextDouble() < _mutationRate)
            state.Herbivores += _rand.Next(-25, 26);

        if (_rand.NextDouble() < _mutationRate)
            state.Carnivores += _rand.Next(-10, 11);

        int plantGrowth = (int)
        (
            PlantGrowthRate * state.Plants -
            PlantConsumptionRate *
            state.Plants *
            state.Herbivores
        );

        state.Plants += plantGrowth;
        state.PlantGrowth = plantGrowth;

        int herbivoreChange = (int)
        (
            HerbivoreBirthRate *
            state.Plants *
            state.Herbivores -

            HerbivoreDeathRate *
            state.Herbivores -

            HerbivorePredationRate *
            state.Herbivores *
            state.Carnivores
        );

        state.Herbivores += herbivoreChange;

        int carnivoreChange = (int)
        (
            CarnivoreBirthRate *
            state.Herbivores *
            state.Carnivores -

            CarnivoreDeathRate *
            state.Carnivores
        );

        state.Carnivores += carnivoreChange;

        state.EventName = string.Empty;

        if (_rand.NextDouble() < 0.05)
        {
            state.Plants = (int)(state.Plants * 0.70);
            state.EventName = "Drought";

            _logger.LogInformation(
                "Environmental Event: Drought");
        }

        if (_rand.NextDouble() < 0.05)
        {
            state.Herbivores += _rand.Next(30, 70);
            state.EventName = "Herbivore Boom";

            _logger.LogInformation(
                "Environmental Event: Herbivore Boom");
        }

        state.Plants = Math.Max(100, state.Plants);
        state.Herbivores = Math.Max(50, state.Herbivores);
        state.Carnivores = Math.Max(5, state.Carnivores);

        state.Carnivores = Math.Min(
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
        }
        public void RunGeneration()
{
    try
    {
        var stopwatch = Stopwatch.StartNew();

        var newPopulation = new List<SimulationState>();

        for (int i = 0; i < _populationSize; i++)
        {
            var parent1 = SelectParent();
            var parent2 = SelectParent();

            var child = Crossover(parent1, parent2);

            Mutate(child);

            child.Generation = _generationCounter + 1;

            newPopulation.Add(child);
        }

        lock (_lock)
        {
            _population.Clear();
            _population.AddRange(newPopulation);
        }

        _generationCounter++;

        stopwatch.Stop();

        double averageFitness =
            _population.Any()
                ? _population.Average(Fitness)
                : 0;

        double bestFitness =
            _population.Any()
                ? _population.Max(Fitness)
                : 0;

        _logger.LogInformation(
            "Generation {Generation} completed in {Time} ms | Avg Fitness = {Average:F2} | Best Fitness = {Best:F2}",
            _generationCounter,
            stopwatch.ElapsedMilliseconds,
            averageFitness,
            bestFitness);

        PrintPopulation();
    }
    catch (Exception ex)
    {
        _logger.LogError(
            ex,
            "Error while running simulation generation.");
    }
}

private void PrintPopulation()
{
    lock (_lock)
    {
        foreach (var state in _population)
        {
            _logger.LogDebug(
                "Generation:{Generation} Plants:{Plants} Herbivores:{Herbivores} Carnivores:{Carnivores} Fitness:{Fitness:F2}",
                state.Generation,
                state.Plants,
                state.Herbivores,
                state.Carnivores,
                Fitness(state));
        }
    }
}

public void StartNewSimulation()
{
    _generationCounter = 0;

    InitializePopulation();

    _logger.LogInformation(
        "Simulation restarted successfully.");
}

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

public IEnumerable<SimulationState> GetPopulation()
{
    lock (_lock)
    {
        return _population
            .Select(state => state.Clone())
            .ToList();
    }
}

public object GetStatistics()
{
    lock (_lock)
    {
        return new
        {
            Generation = _generationCounter,

            PopulationSize = _population.Count,

            AverageFitness = _population.Any()
                ? _population.Average(Fitness)
                : 0,

            BestFitness = _population.Any()
                ? _population.Max(Fitness)
                : 0,

            BestState = GetBestState()
        };
    }
}
