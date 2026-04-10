using FireholdeUnderAttack.Cards;
using FireholdeUnderAttack.Commands;
using FireholdeUnderAttack.Constants;
using FireholdeUnderAttack.Events;
using FireholdeUnderAttack.GameEngine.Saga;
using static FireholdeUnderAttack.GameEngine.GameStateType;

namespace FireholdeUnderAttack.GameEngine.Sagas;

internal static class UseCardCommandSaga
{
    public static readonly CommandSaga<UseCardCommand> Saga = new CommandSaga<UseCardCommand>()
        .WhenIn(PlayerTurn)
        .Validate(PlayerExists)
        .Validate(IsActivePlayer)
        .Validate(CardInInventory)
        .Validate(CardIsActive)
        .Validate(TargetProvidedIfRequired)
        .Execute(ApplyEffect)
        .Execute(RemoveFromInventory)
        .Execute(DetermineNextState)
        .Emit(CardUsed)
        .Emit(VillainDefeated, when: (_, _, ctx) => ctx.Get<bool>("villainDefeated"))
        .TransitionTo((_, _, ctx) => ctx.Get<GameStateType>("nextState"));

    private static bool PlayerExists(UseCardCommand cmd, GameState state) =>
        state.Players.Any(p => p.PlayerId == cmd.PlayerId);

    private static bool IsActivePlayer(UseCardCommand cmd, GameState state) =>
        state.TurnMarker?.ActivePlayerId == cmd.PlayerId;

    private static bool CardInInventory(UseCardCommand cmd, GameState state)
    {
        var player = state.Players.First(p => p.PlayerId == cmd.PlayerId);
        return player.Inventory.Contains(cmd.CardId);
    }

    private static bool CardIsActive(UseCardCommand cmd, GameState state) =>
        CardCatalog.All.TryGetValue(cmd.CardId, out var card) && card.ActiveEffect != null;

    private static bool TargetProvidedIfRequired(UseCardCommand cmd, GameState state) =>
        !CardCatalog.All.TryGetValue(cmd.CardId, out var card)
            ? false
            : card.TargetType == CardTargetType.None || cmd.TargetId.HasValue;

    private static void ApplyEffect(UseCardCommand cmd, GameState state, SagaContext ctx)
    {
        var card = CardCatalog.All[cmd.CardId];
        card.ActiveEffect!.Invoke(state, cmd.PlayerId, cmd.TargetId);
    }

    private static void RemoveFromInventory(UseCardCommand cmd, GameState state, SagaContext ctx)
    {
        var player = state.Players.First(p => p.PlayerId == cmd.PlayerId);
        player.Inventory.Remove(cmd.CardId);
    }

    private static void DetermineNextState(UseCardCommand cmd, GameState state, SagaContext ctx)
    {
        var villainDefeated = state.Villain is { CurrentHealth: <= 0 };
        ctx.Set("villainDefeated", villainDefeated);
        ctx.Set("nextState", villainDefeated ? Final : PlayerTurn);
    }

    private static IEvent CardUsed(UseCardCommand cmd, GameState state, SagaContext ctx) =>
        new CardUsedEvent
        {
            GameId = state.GameId,
            PlayerId = cmd.PlayerId,
            CardId = cmd.CardId
        };

    private static IEvent VillainDefeated(UseCardCommand cmd, GameState state, SagaContext ctx) =>
        new VillainDefeatedEvent
        {
            GameId = state.GameId,
            KilledByPlayerId = cmd.PlayerId
        };
}
