using EcosystemSimulation.Models;

namespace EcosystemSimulation.Interfaces
{
    public interface ISimulationService
    {
        int Generation { get; }

        void RunGeneration();

        SimulationState? GetBestState();

        IEnumerable<SimulationState> GetPopulation();

        void StartNewSimulation();

        object GetStatistics();

        double Fitness(SimulationState state);
    }
}
