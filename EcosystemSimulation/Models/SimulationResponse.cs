namespace EcosystemSimulation.Models
{
    public class SimulationResponse
    {
        public int Generation { get; set; }

        public int Plants { get; set; }

        public int Herbivores { get; set; }

        public int Carnivores { get; set; }

        public double Fitness { get; set; }

        public string EventName { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; }
    }
}
