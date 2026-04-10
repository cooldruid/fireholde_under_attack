using FireholdeUnderAttack.Cards;

namespace FireholdeUnderAttack.Catalogs.Cards;

public class Level1CardCatalog
{
    public static readonly IReadOnlyDictionary<string, CardDefinition> Cards =
        new Dictionary<string, CardDefinition>
        {
            ["healing_potion"] = new(
                Id: "healing_potion",
                Name: "Healing Potion",
                Description: "Restore 8 health.",
                Price: 20,
                Level: 1,
                TargetType: CardTargetType.Ally,
                ActiveEffect: (state, playerId, _) =>
                {
                    var player = state.Players.First(p => p.PlayerId == playerId);
                    player.Health = Math.Min(player.Health + 8, player.MaxHealth);
                }
            ),

            ["shrimp"] = new(
                Id: "shrimp",
                Name: "Shrimp",
                Description: "Get 0-50 Gold.",
                Price: 20,
                Level: 1,
                ActiveEffect: (state, playerId, _) =>
                {
                    var player = state.Players.First(p => p.PlayerId == playerId);
                    player.Gold += Random.Shared.Next(0, 51);
                }
            ),

            ["scroll_of_kindle"] = new(
                Id: "scroll_of_kindle",
                Name: "Scroll of Kindle",
                Description: "Deal 5 damage.",
                Price: 20,
                Level: 1,
                TargetType: CardTargetType.Enemy,
                ActiveEffect: (state, playerId, targetId) =>
                {
                    if (targetId == null) return;
                    if (state.Villain?.Id == targetId)
                        state.Villain.CurrentHealth = Math.Max(0, state.Villain.CurrentHealth - 5);
                    else
                    {
                        var enemy = state.Enemies.FirstOrDefault(e => e.Id == targetId);
                        if (enemy != null) enemy.Health = Math.Max(0, enemy.Health - 5);
                    }
                }
            ),

            ["sword"] = new(
                Id: "sword",
                Name: "Sword",
                Description: "Your Attack action deals 1 more damage. (How does that help a wizard?)",
                Price: 20,
                Level: 1,
                OnAcquire: (state, playerId, _) =>
                {
                    var player = state.Players.First(p => p.PlayerId == playerId);
                    player.AttackDamage += 1;
                }
            ),

            ["walking_shoes"] = new(
                Id: "walking_shoes",
                Name: "Walking Shoes",
                Description: "Increase the amount of tiles you move with 1.",
                Price: 20,
                Level: 1,
                OnMove: (state, playerId, _) =>
                {
                    var player = state.Players.First(p => p.PlayerId == playerId);
                    player.CurrentTile = (player.CurrentTile + 1) % state.Board.Tiles.Count;
                }
            ),
        };
}
