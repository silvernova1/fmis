using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace fmis.Hubs
{
	public class PrStatus : Hub
	{
		public async Task SendUpdate(string prNo)
		{
			await Clients.All.SendAsync("ReceiveUpdate", prNo);
		}
	}
}
