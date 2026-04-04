using FireholdeUnderAttack.Cards;
using FireholdeUnderAttack.GameEngine;

namespace FireholdeUnderAttack.Data;

public class Player
{
    public Guid PlayerId { get; set; }
    public int PlayerIndex { get; set; }
    public string PlayerName { get; set; } = "";
    public int CurrentTile { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Gold { get; set; }
    public List<string> Inventory { get; set; } = [];
    public List<string> AvailableActions { get; set; } = [];
    public List<PlayerTrigger> Triggers { get; set; } = [];
    public int ActionsPerTurn { get; set; }
    public int Level { get; set; }

    private Player()
    { }

    public static Player Create(Guid playerId, int index, string name) => new()
    {
        PlayerId = playerId,
        PlayerIndex = index,
        PlayerName = name,
        CurrentTile = 0,
        Health = GameConfig.StartingHealth,
        MaxHealth = GameConfig.StartingMaxHealth,
        Gold = GameConfig.StartingGold,
        AvailableActions = [.. GameConfig.StartingActions],
        ActionsPerTurn = GameConfig.ActionsPerTurn,
        Level = 1
    };
}

public record PlayerTrigger(
    string Id,
    Func<GameState, Guid, bool> Condition,
    CardEffect Effect
);