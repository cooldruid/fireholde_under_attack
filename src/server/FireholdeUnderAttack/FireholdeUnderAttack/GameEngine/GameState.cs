using FireholdeUnderAttack.Cards;
using FireholdeUnderAttack.Constants;
using FireholdeUnderAttack.Data;

namespace FireholdeUnderAttack.GameEngine;

public class GameState
{
    public Guid GameId { get; set; }
    public Guid OwnerId { get; set; }
    public int SequenceNumber { get; set; }
    public List<Player> Players { get; set; } = [];
    public GameStateType State { get; set; }
    public required Board Board { get; set; }
    public required Villain Villain { get; set; }
    public Guid? ActivePlayerId { get; set; }
    public int ActivePlayerIndex { get; set; }
    public int Round { get; set; }

    private GameState() { }

    public static GameState Create(Guid gameOwnerId, string ownerName)
    {
        return new GameState()
        {
            OwnerId = gameOwnerId,
            Players = [Player.Create(gameOwnerId, 0, ownerName)],
            State = GameStateType.Initial,
            Board = Board.Create(GameConfig.Board),
            Villain = Villain.Create(GameConfig.BossStartingHealth)
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