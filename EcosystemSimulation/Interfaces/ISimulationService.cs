public interface ISimulationService
{
    void RunGeneration();

    SimulationState? GetBestState();

    IEnumerable<SimulationState> GetPopulation();

    void StartNewSimulation();
}
