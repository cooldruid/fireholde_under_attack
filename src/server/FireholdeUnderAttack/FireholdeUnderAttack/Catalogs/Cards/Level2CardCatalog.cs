using FireholdeUnderAttack.Cards;

namespace FireholdeUnderAttack.Catalogs.Cards;

public class Level2CardCatalog
{
    public static readonly IReadOnlyDictionary<string, CardDefinition> Cards =
        new Dictionary<string, CardDefinition>
        {
            ["glasses_of_negotiation"] = new(
                Id: "glasses_of_negotiation",
                Name: "Glasses of Negotiation",
                Description: "When you step on a Quest tile, choose one of 3 quests instead of just getting one.",
                Price: 40,
                Level: 2
                // effect applied by quest system — not yet implemented
            ),

            ["heart_shaped_shield"] = new(
                Id: "heart_shaped_shield",
                Name: "Heart-Shaped Shield",
                Description: "Increase your current and maximum health by 15.",
                Price: 40,
                Level: 2,
                OnAcquire: (state, playerId, _) =>
                {
                    var player = state.Players.First(p => p.PlayerId == playerId);
                    player.MaxHealth += 15;
                    player.Health += 15;
                }
            ),

            ["majestic_trinket_tablet"] = new(
                Id: "majestic_trinket_tablet",
                Name: "Majestic Trinket Tablet",
                Description: "Does nothing? Has some cool trinket slots for a sword, shield and crown though.",
                Price: 40,
                Level: 2
            ),

            ["shiny_crystal"] = new(
                Id: "shiny_crystal",
                Name: "Shiny Crystal",
                Description: "You can now sell cards for 50% their price in shops.",
                Price: 40,
                Level: 2,
                OnAcquire: (state, playerId, _) =>
                {
                    var player = state.Players.First(p => p.PlayerId == playerId);
                    player.CanSellCards = true;
                }
            ),
        };
}
