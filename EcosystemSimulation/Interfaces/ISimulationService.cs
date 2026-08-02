using EcosystemSimulation.Models;

namespace EcosystemSimulation.Interfaces
{
    public interface ISimulationService
    {
        void RunGeneration();

        SimulationState? GetBestState();

        IEnumerable<SimulationState> GetPopulation();

        void StartNewSimulation();

        object GetStatistics();

        int Generation { get; }
    }
}
