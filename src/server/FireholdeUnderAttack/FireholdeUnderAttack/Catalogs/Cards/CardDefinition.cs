using FireholdeUnderAttack.GameEngine;
using FireholdeUnderAttack.GameEngine.Saga;

namespace FireholdeUnderAttack.Cards;

public delegate void CardEffect(GameState state, Guid playerId);

public record CardDefinition(
    string Id,
    string Name,
    string Description,
    int Price,
    int Level,
    CardUsage Usage,
    CardEffect Effect,
    PassiveTrigger? PassiveTrigger = null
);

public enum CardUsage { Active, Passive, Both }

public enum PassiveTrigger { OnMove, OnDamage, OnRoundStart }
