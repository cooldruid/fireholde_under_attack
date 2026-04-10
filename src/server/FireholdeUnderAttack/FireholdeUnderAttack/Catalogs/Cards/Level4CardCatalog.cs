using FireholdeUnderAttack.Cards;
using FireholdeUnderAttack.Constants;

namespace FireholdeUnderAttack.Catalogs.Cards;

public class Level4CardCatalog
{
    public static readonly IReadOnlyDictionary<string, CardDefinition> Cards =
        new Dictionary<string, CardDefinition>
        {
            ["portal_of_things"] = new(
                Id: "portal_of_things",
                Name: "Portal of Things",
                Description: "Get 5 random level 2 cards. Tons of junk!",
                Price: 110,
                Level: 4,
                ActiveEffect: (state, playerId, _) =>
                {
                    var player = state.Players.First(p => p.PlayerId == playerId);
                    var cards = CardCatalog.All.Values
                        .Where(c => c.Level == 2)
                        .OrderBy(_ => Random.Shared.Next())
                        .Take(5)
                        .Select(c => c.Id);
                    foreach (var cardId in cards)
                        player.Inventory.Add(cardId);
                }
            ),

            ["punch_of_midas"] = new(
                Id: "punch_of_midas",
                Name: "Punch Of Midas",
                Description: "Spend all your Gold, deal that much damage.",
                Price: 110,
                Level: 4,
                TargetType: CardTargetType.Enemy,
                ActiveEffect: (state, playerId, targetId) =>
                {
                    var player = state.Players.First(p => p.PlayerId == playerId);
                    var damage = player.Gold;
                    player.Gold = 0;
                    if (targetId == null) return;
                    if (state.Villain?.Id == targetId)
                        state.Villain.CurrentHealth = Math.Max(0, state.Villain.CurrentHealth - damage);
                    else
                    {
                        var enemy = state.Enemies.FirstOrDefault(e => e.Id == targetId);
                        if (enemy != null) enemy.Health = Math.Max(0, enemy.Health - damage);
                    }
                }
            ),

            ["heart_shaped_crown"] = new(
                Id: "heart_shaped_crown",
                Name: "Heart-Shaped Crown",
                Description: "Increase your current and maximum health by 30.",
                Price: 110,
                Level: 4,
                OnAcquire: (state, playerId, _) =>
                {
                    var player = state.Players.First(p => p.PlayerId == playerId);
                    player.MaxHealth += 30;
                    player.Health += 30;
                }
            ),

            ["majestic_shield_trinket"] = new(
                Id: "majestic_shield_trinket",
                Name: "Majestic Shield Trinket",
                Description: "You take 2 less damage from all sources. Fits right into the tablet!",
                Price: 110,
                Level: 4,
                OnAcquire: (state, playerId, _) =>
                {
                    var player = state.Players.First(p => p.PlayerId == playerId);
                    player.DamageReduction += 2;
                }
            ),

            ["good_juju_totem"] = new(
                Id: "good_juju_totem",
                Name: "Good Juju Totem",
                Description: "The next time you die, raise again with 15 health.",
                Price: 110,
                Level: 4,
                OnDeath: (state, playerId, _) =>
                {
                    var player = state.Players.First(p => p.PlayerId == playerId);
                    player.Health = 15;
                    player.Inventory.Remove("good_juju_totem");
                }
            ),
        };
}
