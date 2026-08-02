using Microsoft.EntityFrameworkCore;
using EcosystemSimulation.Models;

namespace EcosystemSimulation.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<SimulationState> SimulationStates { get; set; }
    }
}
