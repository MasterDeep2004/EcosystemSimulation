using EcosystemSimulation.Models;
using Microsoft.EntityFrameworkCore;

namespace EcosystemSimulation.Data
{
    public class SimulationDbContext : DbContext
    {
        public SimulationDbContext(DbContextOptions<SimulationDbContext> options)
            : base(options)
        {
        }

        public DbSet<SimulationState> SimulationStates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SimulationState>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<SimulationState>()
                .Property(x => x.Timestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            base.OnModelCreating(modelBuilder);
        }
    }
}
