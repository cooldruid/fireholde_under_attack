namespace FireholdeUnderAttack.GameEngine;

public class GameState
{
    public Guid Id { get; set; }
    public List<PlayerState> Players { get; set; } = [];
    public required BoardState Board { get; set; }
}

public class PlayerState
{
    public Guid Id { get; set; }
    public int CurrentTile { get; set; }
    public int Health { get; set; }
}

public class BoardState
{
    public List<TileState> Tiles { get; set; } = [];
}

public class TileState
{
    public int Id { get; set; }
    public string Type { get; set; } = "";
    
}