using FireholdeUnderAttack.GameEngine;

namespace FireholdeUnderAttack.Cards;

public delegate void CardEffect(GameState state, Guid playerId, Guid? targetId);

public record CardDefinition(
    string Id,
    string Name,
    string Description,
    int Price,
    int Level,
    CardTargetType TargetType = CardTargetType.None,
    CardEffect? ActiveEffect = null,
    CardEffect? OnAcquire = null,
    CardEffect? OnMove = null,
    CardEffect? OnAttack = null,
    CardEffect? OnTakeDamage = null,
    CardEffect? OnDeath = null
);

public enum CardTargetType { None, Enemy, Ally }
