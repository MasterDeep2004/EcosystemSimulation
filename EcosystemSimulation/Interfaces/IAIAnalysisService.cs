using EcosystemSimulation.Models;

namespace EcosystemSimulation.Interfaces
{
    public interface IAIAnalysisService
    {
        Task<AIAnalysisResponse> AnalyzeAsync(
            SimulationState state,
            CancellationToken cancellationToken = default);
    }
}
