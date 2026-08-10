using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcosystemSimulation.Models
{
    public class SimulationState
    {
        [Key]
        public int Id { get; set; }

        // Simulation metadata
        public int Generation { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string EventName { get; set; } = string.Empty;


        // Population
        public int Plants { get; set; }

        public int Herbivores { get; set; }

        public int Carnivores { get; set; }


        // Population changes
        public int PlantGrowth { get; set; }

        public int PlantConsumed { get; set; }

        public int HerbivoreBirths { get; set; }

        public int HerbivoreDeaths { get; set; }

        public int CarnivoreBirths { get; set; }

        public int CarnivoreDeaths { get; set; }


        // Environmental Events
        public bool DroughtOccurred { get; set; }

        public bool HerbivoreBoomOccurred { get; set; }


        // Analytics
        [NotMapped]
        public int TotalPopulation =>
            Plants + Herbivores + Carnivores;

        public double FitnessScore { get; set; }


        public double Fitness()
        {
            double balance =
                1.0 /
                (1 +
                 Math.Abs(Plants - Herbivores) +
                 Math.Abs(Herbivores - Carnivores));

            FitnessScore =
                TotalPopulation * balance;

            return FitnessScore;
        }


        public SimulationState Clone()
        {
            return (SimulationState)MemberwiseClone();
        }
    }
}
