using System.ComponentModel.DataAnnotations;

namespace EcosystemSimulation.Models
{
    public class Entity
    {
        [Key]
        public int Id { get; set; }

        public EntityType Type { get; set; }

        public int Population { get; set; }

        public double GrowthRate { get; set; }

        public int ConsumptionRate { get; set; }

        public override string ToString()
        {
            return $"{Type}: {Population}";
        }
    }
}
