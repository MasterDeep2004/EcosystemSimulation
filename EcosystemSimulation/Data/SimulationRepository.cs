using EcosystemSimulation.Interfaces;
using EcosystemSimulation.Models;
using Microsoft.EntityFrameworkCore;

namespace EcosystemSimulation.Data
{
    public class SimulationRepository : ISimulationRepository
    {
        private readonly AppDbContext _context;

        public SimulationRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task SaveAsync(
            SimulationState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(
                    nameof(state));
            }

            state.Id = 0;

            _context.SimulationStates.Add(state);

            await _context.SaveChangesAsync();
        }


        public async Task<List<SimulationState>>
            GetHistoryAsync()
        {
            return await _context.SimulationStates
                .AsNoTracking()
                .OrderByDescending(x => x.Generation)
                .ToListAsync();
        }


        public async Task<SimulationState?>
            GetLatestAsync()
        {
            return await _context.SimulationStates
                .AsNoTracking()
                .OrderByDescending(x => x.Generation)
                .FirstOrDefaultAsync();
        }


        public async Task ClearAsync()
        {
            await _context.Database.ExecuteSqlRawAsync(
                "DELETE FROM SimulationStates");
        }
    }
}
