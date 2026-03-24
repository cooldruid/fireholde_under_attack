using FireholdeUnderAttack.GameEngine;

namespace FireholdeUnderAttack.Cards;

public delegate void CardEffect(GameState state, Guid playerId, Guid? targetId);

public record CardDefinition(
    string Id,
    string Name,
    string Description,
    int Price,
    int Level,
    CardUsage Usage,
    CardEffect Effect,
    CardTargetType TargetType = CardTargetType.None
);

public enum CardUsage { Active, Passive }

public enum CardTargetType { None, Enemy, Ally }
