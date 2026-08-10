using EcosystemSimulation.Models;
using Microsoft.EntityFrameworkCore;

namespace EcosystemSimulation.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(
            DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Entity> Entities { get; set; }

        public DbSet<SimulationState> SimulationStates { get; set; }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entity>()
                .Property(e => e.Type)
                .HasConversion<string>();

            modelBuilder.Entity<SimulationState>()
                .Property(s => s.EventName)
                .HasMaxLength(100);

            base.OnModelCreating(modelBuilder);
        }
    }
}
