using FireholdeUnderAttack.Managers;
using Microsoft.AspNetCore.SignalR;

namespace FireholdeUnderAttack.Hubs;

public class EventHub(GameInstanceManager manager) : Hub
{
    public async Task JoinGame(Guid gameId, Guid playerId)
    {
        var game = manager.Get(gameId.ToString());

        if (game.State.Players.All(p => p.PlayerId != playerId))
            throw new HubException($"Player {playerId} is not in game {gameId}.");

        await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
    }

    public async Task LeaveGame(Guid gameId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId.ToString());
    }
}
