using FireholdeUnderAttack.Cards;

namespace FireholdeUnderAttack.Catalogs.Cards;

public class Level3CardCatalog
{
    public static readonly IReadOnlyDictionary<string, CardDefinition> Cards =
        new Dictionary<string, CardDefinition>
        {
            ["potion_of_major_healing"] = new(
                Id: "potion_of_major_healing",
                Name: "Potion of Major Healing",
                Description: "Restore all your health.",
                Price: 75,
                Level: 3,
                ActiveEffect: (state, playerId, _) =>
                {
                    var player = state.Players.First(p => p.PlayerId == playerId);
                    player.Health = player.MaxHealth;
                }
            ),

            ["scroll_of_fire"] = new(
                Id: "scroll_of_fire",
                Name: "Scroll of Fire",
                Description: "Deal 10 damage. BOOM",
                Price: 75,
                Level: 3,
                TargetType: CardTargetType.Enemy,
                ActiveEffect: (state, playerId, targetId) =>
                {
                    if (targetId == null) return;
                    if (state.Villain?.Id == targetId)
                        state.Villain.CurrentHealth = Math.Max(0, state.Villain.CurrentHealth - 10);
                    else
                    {
                        var enemy = state.Enemies.FirstOrDefault(e => e.Id == targetId);
                        if (enemy != null) enemy.Health = Math.Max(0, enemy.Health - 10);
                    }
                }
            ),

            ["goblin_economics_book"] = new(
                Id: "goblin_economics_book",
                Name: "Goblin Economics Book",
                Description: "Get 20% discount in shops. The art of the deal!",
                Price: 75,
                Level: 3,
                OnAcquire: (state, playerId, _) =>
                {
                    var player = state.Players.First(p => p.PlayerId == playerId);
                    player.ShopDiscountPercent += 20;
                }
            ),

            ["azure_canine"] = new(
                Id: "azure_canine",
                Name: "Azure Canine",
                Description: "Get the Ponder the Orb action: You can give a card to another player without being on the Orb tile.",
                Price: 75,
                Level: 3
                // effect applied by action system — not yet implemented
            ),

            ["majestic_sword_trinket"] = new(
                Id: "majestic_sword_trinket",
                Name: "Majestic Sword Trinket",
                Description: "Your attacks deal +2 damage. Fits right into the tablet!",
                Price: 75,
                Level: 3,
                OnAcquire: (state, playerId, _) =>
                {
                    var player = state.Players.First(p => p.PlayerId == playerId);
                    player.AttackDamage += 2;
                }
            ),

            ["armor_of_irrelevance"] = new(
                Id: "armor_of_irrelevance",
                Name: "Armor of Irrelevance",
                Description: "Take 2 less damage from non-villain enemies. Just not important enough.",
                Price: 75,
                Level: 3,
                OnAcquire: (state, playerId, _) =>
                {
                    var player = state.Players.First(p => p.PlayerId == playerId);
                    player.EnemyDamageReduction += 2;
                }
            ),
        };
}
