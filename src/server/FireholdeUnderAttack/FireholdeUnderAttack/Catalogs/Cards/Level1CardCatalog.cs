using FireholdeUnderAttack.Cards;

namespace FireholdeUnderAttack.Catalogs.Cards;

public class Level1CardCatalog
{
    public static readonly IReadOnlyDictionary<string, CardDefinition> Cards =
        new Dictionary<string, CardDefinition>
        {
            // Consumable
            ["healing_potion"] = new(
                Id: "healing_potion",
                Name: "Healing Potion",
                Description: "Restore 8 health.",
                Price: 20,
                Level: 1,
                Usage: CardUsage.Active,
                TargetType: CardTargetType.Ally,
                Effect: (state, playerId, _) =>
                {
                    var player = state.Players.First(p => p.PlayerId == playerId);
                    player.Health += 8;
                }
            ),
            
            ["shrimp"] = new(
                Id: "shrimp",
                Name: "Shrimp",
                Description: "Get 0-50 Gold",
                Price: 20,
                Level: 1,
                Usage: CardUsage.Active,
                Effect: (state, playerId, _) =>
                {
                    var gold = Random.Shared.Next(0, 51);
                    var player = state.Players.First(p => p.PlayerId == playerId);
                    player.Gold += gold;
                }
            ),
            
            ["scroll_of_kindle"] = new(
                Id: "scroll_of_kindle",
                Name: "Scroll of Kindle",
                Description: "Deal 5 damage",
                Price: 20,
                Level: 1,
                Usage: CardUsage.Active,
                TargetType: CardTargetType.Enemy,
                Effect: (state, playerId, targetId) =>
                {
                    
                }
            ),

            // Passive
            ["sword"] = new(
                Id: "sword",
                Name: "Sword",
                Description: "Your Attack action deals 1 more damage. (How does that help a wizard?)",
                Price: 20,
                Level: 1,
                Usage: CardUsage.Passive,
                Effect: (state, playerId, targetId) =>
                {

                }
            ),
            
            ["walking_shoes"] = new(
                Id: "walking_shoes",
                Name: "Walking Shoes",
                Description: "Increase the amount of tiles you move with 1",
                Price: 20,
                Level: 1,
                Usage: CardUsage.Passive,
                Effect: (state, playerId, targetId) =>
                {

                }
            ),
        };
}