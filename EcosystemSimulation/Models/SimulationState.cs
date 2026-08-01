public class SimulationState
{
    public int Id { get; set; }

    public int Plants { get; set; }

    public int Herbivores { get; set; }

    public int Carnivores { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public int PlantGrowth { get; set; }
    public int PlantConsumed { get; set; }

    public int HerbivoreBirths { get; set; }
    public int HerbivoreDeaths { get; set; }

    public int CarnivoreBirths { get; set; }
    public int CarnivoreDeaths { get; set; }

    // Useful metric
    public int TotalPopulation =>
        Plants + Herbivores + Carnivores;

    public double Fitness()
    {
        double balance =
            1.0 /
            (1 +
             Math.Abs(Plants - Herbivores) +
             Math.Abs(Herbivores - Carnivores));

        return TotalPopulation * balance;
    }

    public SimulationState Clone()
    {
        return (SimulationState)MemberwiseClone();
    }
}
