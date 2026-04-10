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
    public int ActionsPerTurn { get; set; }
    public int Level { get; set; }

    // Combat modifiers — written by OnAcquire passives, read by combat/damage systems
    public int AttackDamage { get; set; }
    public int DamageReduction { get; set; }
    public int EnemyDamageReduction { get; set; }

    // Shop modifiers
    public int ShopDiscountPercent { get; set; }
    public bool CanSellCards { get; set; }

    // Status flags
    public bool IsInvincible { get; set; }

    // Class-granted modifiers
    public int AttackTargets { get; set; } = 1;

    private Player() { }

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
        Level = 1,
        AttackDamage = GameConfig.BaseAttackDamage
    };
}
