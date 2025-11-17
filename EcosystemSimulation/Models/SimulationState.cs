namespace EcosystemSimulation.Models
{
    public class SimulationState
    {
        public int Id { get; set; }

        // Ecosystem populations
        public int Plants { get; set; }
        public int Herbivores { get; set; }
        public int Carnivores { get; set; }

        public DateTime Timestamp { get; set; }

        // Detailed tracking (optional, useful for logging)
        public int PlantGrowth { get; set; }
        public int PlantConsumed { get; set; }
        public int HerbivoreBirths { get; set; }
        public int HerbivoreDeaths { get; set; }
        public int CarnivoreBirths { get; set; }
        public int CarnivoreDeaths { get; set; }

        // Fitness score for GA
        public double Fitness()
        {
            // Balance of ecosystem: penalize extremes
            double balanceScore = 1.0 / (1 + Math.Abs(Plants - Herbivores) + Math.Abs(Herbivores - Carnivores));
            double totalPopulation = Plants + Herbivores + Carnivores;
            return totalPopulation * balanceScore;
        }

        // Optional: simple clone for GA operations
        public SimulationState Clone()
        {
            return new SimulationState
            {
                Id = this.Id,
                Plants = this.Plants,
                Herbivores = this.Herbivores,
                Carnivores = this.Carnivores,
                Timestamp = this.Timestamp,
                PlantGrowth = this.PlantGrowth,
                PlantConsumed = this.PlantConsumed,
                HerbivoreBirths = this.HerbivoreBirths,
                HerbivoreDeaths = this.HerbivoreDeaths,
                CarnivoreBirths = this.CarnivoreBirths,
                CarnivoreDeaths = this.CarnivoreDeaths
            };
        }
    }
}
