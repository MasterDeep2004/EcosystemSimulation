using EcosystemSimulation.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcosystemSimulation.Services
{
    /// <summary>
    /// Handles the predator–prey–plant ecosystem simulation
    /// using Genetic Algorithm concepts combined with Lotka–Volterra inspired population dynamics.
    /// </summary>
    public class SimulationService
    {
        private int _generationCounter = 0;
        private readonly int _populationSize = 20;
        private readonly List<SimulationState> _population = new();
        private readonly Random _rand = new();

        public SimulationService()
        {
            InitializePopulation();
        }

        private void InitializePopulation()
        {
            _population.Clear();
            for (int i = 0; i < _populationSize; i++)
            {
                var state = new SimulationState
                {
                    Id = i,
                    Plants = _rand.Next(800, 1200),
                    Herbivores = _rand.Next(200, 400),
                    Carnivores = _rand.Next(30, 80),
                    Timestamp = DateTime.UtcNow
                };
                _population.Add(state);
            }

            Console.WriteLine("[INIT POPULATION]");
            PrintPopulation();
        }

        /// <summary>
        /// Fitness function for a simulation state
        /// </summary>
        public double Fitness(SimulationState s)
        {
            if (s == null) return 0;
            try
            {
                double balanceScore = 1.0 / (1 + Math.Abs(s.Plants - s.Herbivores) + Math.Abs(s.Herbivores - s.Carnivores));
                double diversityScore = (s.Plants > 200 && s.Herbivores > 50 && s.Carnivores > 10) ? 1.2 : 0.8;
                double totalPopulation = s.Plants + s.Herbivores + s.Carnivores;
                double noise = 0.9 + _rand.NextDouble() * 0.2;
                return totalPopulation * balanceScore * diversityScore * noise;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Fitness] Error computing fitness: {ex.Message}");
                return 0;
            }
        }

        private SimulationState SelectParent()
        {
            int tournamentSize = 3;
            var candidates = new List<SimulationState>();
            for (int i = 0; i < tournamentSize; i++)
                candidates.Add(_population[_rand.Next(_population.Count)]);
            return candidates.OrderByDescending(Fitness).FirstOrDefault();
        }

        private SimulationState Crossover(SimulationState p1, SimulationState p2)
        {
            if (p1 == null || p2 == null) return new SimulationState { Plants = 100, Herbivores = 50, Carnivores = 5, Timestamp = DateTime.UtcNow };

            return new SimulationState
            {
                Id = 0,
                Plants = Math.Max(100, (p1.Plants + p2.Plants) / 2 + _rand.Next(-30, 31)),
                Herbivores = Math.Max(50, (p1.Herbivores + p2.Herbivores) / 2 + _rand.Next(-20, 21)),
                Carnivores = Math.Max(5, (p1.Carnivores + p2.Carnivores) / 2 + _rand.Next(-10, 11)),
                Timestamp = DateTime.UtcNow
            };
        }

        private void Mutate(SimulationState s)
        {
            if (s == null) return;
            try
            {
                double mutationRate = 0.25;
                if (_rand.NextDouble() < mutationRate) s.Plants += _rand.Next(-50, 51);
                if (_rand.NextDouble() < mutationRate) s.Herbivores += _rand.Next(-25, 26);
                if (_rand.NextDouble() < mutationRate) s.Carnivores += _rand.Next(-10, 11);

                // Lotka–Volterra dynamics
                int plantGrowth = (int)(0.15 * s.Plants - 0.002 * s.Plants * s.Herbivores);
                s.Plants = Math.Max(100, s.Plants + plantGrowth);

                int herbivoreChange = (int)(0.003 * s.Plants * s.Herbivores - 0.08 * s.Herbivores - 0.002 * s.Herbivores * s.Carnivores);
                s.Herbivores = Math.Max(50, s.Herbivores + herbivoreChange);

                int carnivoreChange = (int)(0.0015 * s.Herbivores * s.Carnivores - 0.06 * s.Carnivores);
                s.Carnivores = Math.Max(5, s.Carnivores + carnivoreChange);

                // Cap carnivores
                s.Carnivores = Math.Min(s.Carnivores, (int)(0.4 * s.Herbivores));

                // Random environmental events
                if (_rand.NextDouble() < 0.05)
                {
                    Console.WriteLine("⚡ Drought reduces plants!");
                    s.Plants = (int)(s.Plants * 0.7);
                }
                if (_rand.NextDouble() < 0.05)
                {
                    Console.WriteLine("🐇 Herbivore boom!");
                    s.Herbivores += _rand.Next(30, 70);
                }

                // Ensure minimum floors
                s.Plants = Math.Max(100, s.Plants);
                s.Herbivores = Math.Max(50, s.Herbivores);
                s.Carnivores = Math.Max(5, s.Carnivores);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Mutate] Error: {ex.Message}");
            }
        }

        public void RunGeneration()
        {
            _generationCounter++;
            var newPopulation = new List<SimulationState>();
            try
            {
                while (newPopulation.Count < _populationSize)
                {
                    var p1 = SelectParent();
                    var p2 = SelectParent();
                    var child = Crossover(p1, p2);
                    Mutate(child);
                    newPopulation.Add(child);
                }

                _population.Clear();
                _population.AddRange(newPopulation);

                Console.WriteLine($"[GENERATION {_generationCounter}]");
                PrintPopulation();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RunGeneration] Error: {ex.Message}");
            }
        }

        private void PrintPopulation()
        {
            foreach (var s in _population)
            {
                Console.WriteLine($"🌱={s.Plants}, 🐇={s.Herbivores}, 🦁={s.Carnivores} | Fitness={Fitness(s):F2}");
            }
        }

        public void StartNewSimulation()
        {
            _generationCounter = 0;
            InitializePopulation();
        }

        public SimulationState GetBestState()
        {
            try
            {
                return _population.OrderByDescending(Fitness).FirstOrDefault() ?? new SimulationState { Plants = 100, Herbivores = 50, Carnivores = 5, Timestamp = DateTime.UtcNow };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GetBestState] Error: {ex.Message}");
                return new SimulationState { Plants = 100, Herbivores = 50, Carnivores = 5, Timestamp = DateTime.UtcNow };
            }
        }

        public IEnumerable<SimulationState> GetPopulation()
        {
            return _population.AsEnumerable();
        }
    }
}
