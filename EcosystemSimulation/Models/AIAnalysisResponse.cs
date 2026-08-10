namespace EcosystemSimulation.Models
{
    public class AIAnalysisResponse
    {
        public int Generation { get; set; }

        public string Summary { get; set; } = string.Empty;

        public string PopulationTrend { get; set; } = string.Empty;

        public string RiskLevel { get; set; } = string.Empty;

        public string Recommendation { get; set; } = string.Empty;

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
}
