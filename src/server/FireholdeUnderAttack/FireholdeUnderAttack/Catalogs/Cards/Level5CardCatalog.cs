using FireholdeUnderAttack.Cards;

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
                Usage: CardUsage.Passive,
                Effect: (state, playerId, targetId) =>
                {
                    
                }
            ),
            
            ["majestic_crown_trinket"] = new(
                Id: "majestic_crown_trinket",
                Name: "Majestic Crown Trinket",
                Description: "If you have the Majestic Trinket Tablet, the Majestic Sword Trinket and the Majestic Shield Trinket, KILL THE VILLAIN!",
                Price: 200,
                Level: 5,
                Usage: CardUsage.Passive,
                Effect: (state, playerId, targetId) =>
                {
                    
                }
            ),
        };
}