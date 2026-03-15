using FireholdeUnderAttack.GameEngine;
using FireholdeUnderAttack.Hubs;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace FireholdeUnderAttack.Managers;

public class GameInstanceManager(IHubContext<EventHub> hubContext)
{
    private readonly Dictionary<string, GameInstance> _gameInstances = [];

    public GameInstance Create(Guid gameOwnerId, string ownerName)
    {
        var instance = new GameInstance(gameOwnerId, ownerName, hubContext);
        _gameInstances.Add(instance.Id.ToString(), instance);
        Log.Information("Created game instance {GameInstanceId}", instance.Id);
        return instance;
    }

    public GameInstance Get(string gameId)
    {
        var exists = _gameInstances.TryGetValue(gameId, out var gameInstance);

        if (!exists || gameInstance == null)
            throw new ArgumentException($"Game instance {gameId} doesn't exist");

        return gameInstance;
    }

    public void Delete(string gameId)
    {
        _gameInstances.Remove(gameId);
        Log.Information("Deleted game instance {GameInstanceId}", gameId);
    }
}
