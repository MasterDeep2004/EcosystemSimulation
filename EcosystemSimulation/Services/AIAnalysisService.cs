using EcosystemSimulation.Interfaces;
using EcosystemSimulation.Models;
using OpenAI.Responses;

namespace EcosystemSimulation.Services
{
    public class AIAnalysisService : IAIAnalysisService
    {
        private readonly ResponsesClient _client;
        private readonly ILogger<AIAnalysisService> _logger;

        public AIAnalysisService(
            ResponsesClient client,
            ILogger<AIAnalysisService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<AIAnalysisResponse> AnalyzeAsync(
            SimulationState state,
            CancellationToken cancellationToken = default)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            var prompt = $"""
                Analyze this simulated ecosystem as an
                environmental analyst.

                Generation: {state.Generation}

                Plants: {state.Plants}

                Herbivores: {state.Herbivores}

                Carnivores: {state.Carnivores}

                Plant Growth: {state.PlantGrowth}

                Plant Consumed: {state.PlantConsumed}

                Herbivore Births: {state.HerbivoreBirths}

                Herbivore Deaths: {state.HerbivoreDeaths}

                Carnivore Births: {state.CarnivoreBirths}

                Carnivore Deaths: {state.CarnivoreDeaths}

                Environmental Event: {state.EventName}

                Fitness Score: {state.FitnessScore:F2}

                Give a short practical analysis.

                Include:
                1. Overall summary
                2. Population trend
                3. Risk level: Low, Medium, or High
                4. One recommendation

                Keep the response suitable for a
                software engineering project dashboard.
                """;

            try
            {
                var response =
                    await _client.CreateResponseAsync(
                        prompt,
                        cancellationToken);

                var analysis =
                    response.Value.GetOutputText();

                return new AIAnalysisResponse
                {
                    Generation = state.Generation,
                    Summary = analysis,
                    PopulationTrend =
                        DetermineTrend(state),
                    RiskLevel =
                        DetermineRisk(state),
                    Recommendation =
                        GenerateRecommendation(state),
                    GeneratedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "AI ecosystem analysis failed.");

                return new AIAnalysisResponse
                {
                    Generation = state.Generation,
                    Summary =
                        "AI analysis is temporarily unavailable.",
                    PopulationTrend =
                        DetermineTrend(state),
                    RiskLevel =
                        DetermineRisk(state),
                    Recommendation =
                        GenerateRecommendation(state),
                    GeneratedAt = DateTime.UtcNow
                };
            }
        }

        private static string DetermineTrend(
            SimulationState state)
        {
            if (state.HerbivoreDeaths >
                state.HerbivoreBirths)
            {
                return "Herbivore population is declining.";
            }

            if (state.HerbivoreBirths >
                state.HerbivoreDeaths)
            {
                return "Herbivore population is increasing.";
            }

            return "Herbivore population is relatively stable.";
        }

        private static string DetermineRisk(
            SimulationState state)
        {
            if (state.Plants < 300 ||
                state.Herbivores < 50 ||
                state.Carnivores < 10)
            {
                return "High";
            }

            if (state.Plants < 500 ||
                state.Herbivores < 100)
            {
                return "Medium";
            }

            return "Low";
        }

        private static string GenerateRecommendation(
            SimulationState state)
        {
            if (state.Plants < 300)
            {
                return
                    "Monitor plant depletion and herbivore pressure.";
            }

            if (state.Herbivores > state.Plants)
            {
                return
                    "Monitor herbivore growth because plant resources may become limited.";
            }

            if (state.Carnivores < 10)
            {
                return
                    "Monitor the carnivore population for possible predator imbalance.";
            }

            return
                "Continue monitoring population changes over the next generations.";
        }
    }
}
