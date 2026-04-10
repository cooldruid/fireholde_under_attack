using FireholdeUnderAttack.Cards;
using FireholdeUnderAttack.Data;

namespace FireholdeUnderAttack.Catalogs.Cards;

public class Level5CardCatalog
{
    public static readonly IReadOnlyDictionary<string, CardDefinition> Cards =
        new Dictionary<string, CardDefinition>
        {
            ["the_snail"] = new(
                Id: "the_snail",
                Name: "THE SNAIL",
                Description: "You can no longer take damage or die. An unmoving snail spawns on a random tile. When you step on that tile, die and destroy this item. IT'S COMING",
                Price: 200,
                Level: 5,
                OnAcquire: (state, playerId, _) =>
                {
                    var player = state.Players.First(p => p.PlayerId == playerId);
                    player.IsInvincible = true;
                    state.Enemies.Add(Enemy.Create(state.Board, name: "The Snail"));
                },
                OnMove: (state, playerId, _) =>
                {
                    var player = state.Players.First(p => p.PlayerId == playerId);
                    var snail = state.Enemies.FirstOrDefault(e => e.Name == "The Snail");
                    if (snail == null || player.CurrentTile != snail.Position)
                        return;
                    
                    player.IsInvincible = false;
                    player.Health = 0;
                    player.Inventory.Remove("the_snail");
                    state.Enemies.Remove(snail);
                }
            ),

            ["majestic_crown_trinket"] = new(
                Id: "majestic_crown_trinket",
                Name: "Majestic Crown Trinket",
                Description: "If you have the Majestic Trinket Tablet, the Majestic Sword Trinket and the Majestic Shield Trinket, KILL THE VILLAIN!",
                Price: 200,
                Level: 5,
                OnAcquire: (state, playerId, _) =>
                {
                    var player = state.Players.First(p => p.PlayerId == playerId);
                    if (state.Villain != null
                        && player.Inventory.Contains("majestic_trinket_tablet")
                        && player.Inventory.Contains("majestic_sword_trinket")
                        && player.Inventory.Contains("majestic_shield_trinket"))
                    {
                        state.Villain.CurrentHealth = 0;
                    }
                }
            ),
        };
}
