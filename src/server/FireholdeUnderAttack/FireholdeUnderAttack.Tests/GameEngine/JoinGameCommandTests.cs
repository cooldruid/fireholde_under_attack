using FireholdeUnderAttack.Commands;
using FireholdeUnderAttack.Events;
using FireholdeUnderAttack.GameEngine;

namespace FireholdeUnderAttack.Tests.GameEngine;

public class JoinGameCommandTests
{
    private static readonly Guid GameId = Guid.NewGuid();
    private static readonly Guid OwnerId = Guid.NewGuid();
    private static readonly Guid JoiningPlayerId = Guid.NewGuid();

    private static GameState BuildState(GameStateType stateType)
    {
        var state = GameState.Create(OwnerId, "Owner");
        state.GameId = GameId;
        state.State = stateType;
        return state;
    }

    private static JoinGameCommand BuildCommand(Guid? playerId = null) => new()
    {
        PlayerId = playerId ?? JoiningPlayerId,
        PlayerName = "Joining Player"
    };

    // ── Broad flow tests ──────────────────────────────────────────────────────

    [Fact]
    public void GameStateMachine_OnJoinGameCommand_AddsPlayerToLobby()
    {
        // Arrange
        var state = BuildState(GameStateType.Initial);

        // Act
        var events = new GameStateMachine(state).Handle(BuildCommand());

        // Assert
        var joined = Assert.IsType<PlayerJoinedEvent>(Assert.Single(events));
        Assert.Equal(JoiningPlayerId, joined.PlayerId);
        Assert.Equal("Joining Player", joined.PlayerName);
        Assert.Equal(GameId, joined.GameId);

        Assert.Equal(GameStateType.Initial, state.State);
        Assert.Equal(2, state.Players.Count);
        Assert.Contains(state.Players, p => p.PlayerId == JoiningPlayerId && p.PlayerName == "Joining Player");
    }

    // ── Rejection scenarios ───────────────────────────────────────────────────

    [Theory]
    [InlineData(GameStateType.PlayerTurn)]
    [InlineData(GameStateType.VillainTurn)]
    [InlineData(GameStateType.Final)]
    public void GameStateMachine_OnJoinGameCommand_WhenGameAlreadyStarted_Rejects(GameStateType stateType)
    {
        // Arrange
        var state = BuildState(stateType);

        // Act
        var events = new GameStateMachine(state).Handle(BuildCommand());

        // Assert
        var rejected = Assert.Single(events);
        Assert.IsType<CommandRejectedEvent>(rejected);
    }

    [Fact]
    public void GameStateMachine_OnJoinGameCommand_WhenPlayerAlreadyInGame_Rejects()
    {
        // Arrange
        var state = BuildState(GameStateType.Initial);
        var command = BuildCommand(playerId: OwnerId); // owner is already in the game

        // Act
        var events = new GameStateMachine(state).Handle(command);

        // Assert
        var rejected = Assert.IsType<CommandRejectedEvent>(Assert.Single(events));
        Assert.Equal(GameId, rejected.GameId);
    }

    [Fact]
    public void GameStateMachine_OnJoinGameCommand_WhenGameIsFull_Rejects()
    {
        // Arrange
        var state = BuildState(GameStateType.Initial);
        state.Players.Add(new PlayerState { PlayerId = Guid.NewGuid(), CurrentTile = 1, Health = 50 });
        state.Players.Add(new PlayerState { PlayerId = Guid.NewGuid(), CurrentTile = 1, Health = 50 });
        state.Players.Add(new PlayerState { PlayerId = Guid.NewGuid(), CurrentTile = 1, Health = 50 });
        // 4 players total now (owner + 3 added)

        // Act
        var events = new GameStateMachine(state).Handle(BuildCommand());

        // Assert
        var rejected = Assert.Single(events);
        Assert.IsType<CommandRejectedEvent>(rejected);
    }
}
