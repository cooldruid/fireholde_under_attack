using FireholdeUnderAttack.Data;

namespace FireholdeUnderAttack;

public static class GameConfig
{
    public const int StartingHealth = 50;
    public const int StartingMaxHealth = 50;
    public const int StartingGold = 100;
    public const int BossStartingHealth = 500;
    public const int ActionsPerTurn = 3;

    public static readonly IReadOnlyList<string> StartingActions = ["move", "attack"];
    
    public static List<Tile> Board =
    [
        new() { Id = 0, Type = BoardTileType.Start },
        new() { Id = 1, Type = BoardTileType.Empty },
        new() { Id = 2, Type = BoardTileType.Empty },
        new() { Id = 3, Type = BoardTileType.Shop },
        new() { Id = 4, Type = BoardTileType.Empty },
        new() { Id = 5, Type = BoardTileType.Empty },
        new() { Id = 6, Type = BoardTileType.Quest },
        new() { Id = 7, Type = BoardTileType.Empty },
        new() { Id = 8, Type = BoardTileType.Empty },
        new() { Id = 9, Type = BoardTileType.Orb },
        new() { Id = 10, Type = BoardTileType.Empty },
        new() { Id = 11, Type = BoardTileType.Empty },
        new() { Id = 12, Type = BoardTileType.Quest },
        new() { Id = 13, Type = BoardTileType.Empty },
        new() { Id = 14, Type = BoardTileType.Empty },
        new() { Id = 15, Type = BoardTileType.Empty },
        new() { Id = 16, Type = BoardTileType.Shop },
        new() { Id = 17, Type = BoardTileType.Empty },
        new() { Id = 18, Type = BoardTileType.Empty },
        new() { Id = 19, Type = BoardTileType.Empty },
        new() { Id = 20, Type = BoardTileType.Empty },
        new() { Id = 21, Type = BoardTileType.Empty },
        new() { Id = 22, Type = BoardTileType.Shop },
        new() { Id = 23, Type = BoardTileType.Empty },
        new() { Id = 24, Type = BoardTileType.Empty },
        new() { Id = 25, Type = BoardTileType.Empty },
        new() { Id = 26, Type = BoardTileType.Empty },
        new() { Id = 27, Type = BoardTileType.Graveyard },
        new() { Id = 28, Type = BoardTileType.Empty },
        new() { Id = 29, Type = BoardTileType.Empty },
        new() { Id = 30, Type = BoardTileType.Empty },
        new() { Id = 31, Type = BoardTileType.Empty },
        new() { Id = 32, Type = BoardTileType.Empty },
        new() { Id = 33, Type = BoardTileType.Empty },
        new() { Id = 34, Type = BoardTileType.Empty },
        new() { Id = 35, Type = BoardTileType.Empty }
    ];
}
