using EcosystemSimulation.Data;
using EcosystemSimulation.Interfaces;
using EcosystemSimulation.Models;
using Microsoft.EntityFrameworkCore;

namespace EcosystemSimulation.Repositories
{
    public class SimulationRepository : ISimulationRepository
    {
        private readonly SimulationDbContext _context;

        public SimulationRepository(SimulationDbContext context)
        {
            _context = context;
        }

        public async Task SaveAsync(SimulationState state)
        {
            await _context.SimulationStates.AddAsync(state);
            await _context.SaveChangesAsync();
        }

        public async Task<List<SimulationState>> GetHistoryAsync()
        {
            return await _context.SimulationStates
                .OrderBy(x => x.Generation)
                .ToListAsync();
        }

        public async Task<SimulationState?> GetLatestAsync()
        {
            return await _context.SimulationStates
                .OrderByDescending(x => x.Generation)
                .FirstOrDefaultAsync();
        }

        public async Task ClearAsync()
        {
            _context.SimulationStates.RemoveRange(_context.SimulationStates);
            await _context.SaveChangesAsync();
        }
    }
}
