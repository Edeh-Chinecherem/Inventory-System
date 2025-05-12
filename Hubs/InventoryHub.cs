using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace InventorySystem.Hubs
{
    public class InventoryHub : Hub
    {
        // This method can be called from server to notify all clients to refresh their product list
        public async Task NotifyInventoryUpdate()
        {
            await Clients.All.SendAsync("InventoryUpdated");
        }
    }
}
