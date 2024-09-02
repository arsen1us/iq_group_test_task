using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace IQGROUP_test_task
{
    public class ColorThemeHub : Hub
    {
        public List<string> UsersList { get; set; }

        public ColorThemeHub()
        {
            UsersList = new List<string>();
        }

        public async Task Send(string message, string id)
        {
            UsersList.Add(id);

            await this.Clients.Caller.SendAsync("Receive", message, id);
            await this.Clients.Others.SendAsync("Receive1", $"{id} change color theme", "");

            string usersList = JsonSerializer.Serialize(UsersList);
            await this.Clients.All.SendAsync("UsersList", usersList);
        }

        public override async Task OnConnectedAsync()
        {
            //var context = Context.GetHttpContext();
            //byte[] buffer = new byte[1024];
            //var contextBody = context.Request.Body.ReadAsync(buffer);
            //if(contextBody)
            await this.Clients.Others.SendAsync("OnConnection", "new user is connected");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await this.Clients.Others.SendAsync("OnDisconnection", "user id disconnected");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task GetConnectedUsers()
        {
            string usersList = JsonSerializer.Serialize(UsersList);
            await this.Clients.All.SendAsync("UsersList", UsersList);
        }
    }
}
