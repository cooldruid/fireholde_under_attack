using FireholdeUnderAttack.Constants;

namespace FireholdeUnderAttack.GameEngine;

public class GameState
{
    public List<PlayerState> Players { get; set; } = [];
    public GameStateType State { get; set; }
    public required BoardState Board { get; set; }
    public Guid? ActivePlayerId { get; set; }
    public int ActivePlayerIndex { get; set; }
    public int Round { get; set; }

    private GameState()
    { }

    public static GameState Create(Guid gameOwnerId)
    {
        return new GameState()
        {
            Players =
            [
                new()
                {
                    Id = gameOwnerId,
                    CurrentTile = 1,
                    Health = 50
                }
            ],
            State = GameStateType.Initial,
            Board = new()
            {
                Tiles = BoardConstants.Board
            }
        };
    }
}

public enum GameStateType
{
    Initial = 0,
    PlayerTurn = 1,
    VillainTurn = 2,
    Final = 3
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