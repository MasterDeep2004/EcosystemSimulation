using Microsoft.AspNetCore.SignalR;
using EcosystemSimulation.Models;

namespace EcosystemSimulation.Hubs
{
    public class SimulationHub : Hub
    {
        // Optional: Manual trigger from frontend
        public async Task SendManualUpdate(SimulationState state)
        {
            // Sends the provided state to all connected clients
            await Clients.All.SendAsync("ReceiveUpdate", state);
        }

        // Optional: Send a test message to the caller
        public async Task TestConnection()
        {
            await Clients.Caller.SendAsync("ReceiveUpdate", new SimulationState
            {
                Plants = 10,
                Herbivores = 5,
                Carnivores = 2,
                Id = 0,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
