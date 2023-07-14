
using fmis.Data;
using fmis.Models;
using fmis.Models.John;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace fmis.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private MyDbContext dbContext;
        public ChatHub(MyDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public void SendMessage(string user, string message)
        {
            
            Logs logs = new Logs();
            logs.created_name = user;
            logs.created_designation = message;
            dbContext.Logs.Add(logs);
            dbContext.SaveChanges();
            Clients.All.SendAsync("ReceiveMessage", user, message);
        }

    }
}
