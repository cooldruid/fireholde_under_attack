using FireholdeUnderAttack.Constants;

namespace FireholdeUnderAttack.GameEngine;

public class GameState
{
    public Guid GameId { get; set; }
    public Guid OwnerId { get; set; }
    public int SequenceNumber { get; set; }
    public List<PlayerState> Players { get; set; } = [];
    public GameStateType State { get; set; }
    public required BoardState Board { get; set; }
    public Guid? ActivePlayerId { get; set; }
    public int ActivePlayerIndex { get; set; }
    public int Round { get; set; }

    private GameState()
    { }

    public static GameState Create(Guid gameOwnerId, string ownerName)
    {
        return new GameState()
        {
            OwnerId = gameOwnerId,
            Players =
            [
                new()
                {
                    PlayerId = gameOwnerId,
                    PlayerName = ownerName,
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
    public Guid PlayerId { get; set; }
    public string PlayerName { get; set; } = "";
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