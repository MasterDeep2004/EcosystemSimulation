using EcosystemSimulation.Models;

namespace EcosystemSimulation.Interfaces
{
    public interface ISimulationRepository
    {
        Task SaveAsync(SimulationState state);

        Task<List<SimulationState>> GetHistoryAsync();

        Task<SimulationState?> GetLatestAsync();

        Task ClearAsync();
    }
}
