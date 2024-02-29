using ChatWe.Persistance.Context;
using Microsoft.AspNetCore.SignalR;

namespace ChatWe.Hubs
{
    public class ChatHub : Hub
    {
        public readonly IChatWeContext _context;

        public ChatHub(IChatWeContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            var username = Context?.User?.Identity!.Name;
            Context!.Items["UserName"] = username;

            await base.OnConnectedAsync();
        }

        public string GetCurrentUserName()
        {
            return Context?.Items["UserName"]!.ToString()!;
        }

        public string GetCurrentUserId()
        {
            var userId = Context.User.Claims.FirstOrDefault()!.Value.ToString();
            return userId;
        }

        public int GetGroupId(string senderId, string receiverId)
        {
            var existingGroup = _context.Group
            .FirstOrDefault(g => g.Name == $"{senderId}_{receiverId}" || g.Name == $"{receiverId}_{senderId}");
            return existingGroup == null ? 0 : existingGroup.Id;
        }

        public async Task AddToGroup(int groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());
        }

        public async Task SendMessage(string user, string message, int groupId)
        {
            await Clients.Group(groupId.ToString()).SendAsync("ReceiveMessage", user, message);
        }
    }
}