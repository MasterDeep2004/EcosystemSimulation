namespace EcosystemSimulation.Models
{
    public enum EntityType { Plant, Herbivore, Carnivore }

    public class Entity
    {
        public int Id { get; set; }
        public EntityType Type { get; set; }
        public int Population { get; set; }
        // Optional: growth rate per simulation step
        public double GrowthRate { get; set; } = 0;

        // Optional: amount consumed by other entities per step
        public int ConsumptionRate { get; set; } = 0;

        // Constructor for convenience
        public Entity(int id, EntityType type, int population, double growthRate = 0, int consumptionRate = 0)
        {
            Id = id;
            Type = type;
            Population = population;
            GrowthRate = growthRate;
            ConsumptionRate = consumptionRate;
        }
    }
}
