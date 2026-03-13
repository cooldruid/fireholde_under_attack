using Microsoft.AspNetCore.SignalR;

namespace FireholdeUnderAttack.Hubs;

public class EventHub : Hub
{
    public async Task JoinGame(Guid gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
    }

    public async Task LeaveGame(Guid gameId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId.ToString());
    }
}
