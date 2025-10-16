using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Framework.Identity.Data.Helper
{
    public class SignalR : Hub
    {
        public string GetConnectionId() => Context.ConnectionId;
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("newMessage", "anonymous", message);
        }
        public async Task InformAll(object data)
        {
            await Clients.All.SendAsync("MessageAll", "anonymous", data);
        }
        public async Task InformClientByConnectionId(object data, string connectionId)
        {
            await Clients.Client(connectionId).SendAsync("InformClient", data);
        }
        public Task InformGroup(object data, string groupName)
        {
            return Clients.Group(groupName).SendAsync("SendGroup", data);
        }
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
