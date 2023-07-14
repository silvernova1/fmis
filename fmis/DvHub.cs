using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace fmis
{
    public class DvHub : Hub
    {
        public async Task NotifyDvNoExists(string dvNo)
        {
            await Clients.All.SendAsync("DvNoExists", dvNo);
        }
    }
}
