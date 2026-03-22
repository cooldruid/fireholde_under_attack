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
                Usage: CardUsage.Active,
                Effect: (state, playerId) =>
                {
                    
                }
            ),
            
            ["scroll_of_fire"] = new(
                Id: "scroll_of_fire",
                Name: "Scroll of Fire",
                Description: "Deal 10 damage. BOOM",
                Price: 75,
                Level: 3,
                Usage: CardUsage.Active,
                Effect: (state, playerId) =>
                {
                    
                }
            ),
            
            ["goblin_economics_book"] = new(
                Id: "goblin_economics_book",
                Name: "Goblin Economics Book",
                Description: "Get 20% discount in shops. The art of the deal!",
                Price: 75,
                Level: 3,
                Usage: CardUsage.Passive,
                Effect: (state, playerId) =>
                {
                    
                }
            ),
            
            ["azure_canine"] = new(
                Id: "azure_canine",
                Name: "Azure Canine",
                Description: "Get the Ponder the Orb action: You can give a card to another player without being on the Orb tile",
                Price: 75,
                Level: 3,
                Usage: CardUsage.Passive,
                Effect: (state, playerId) =>
                {
                    
                }
            ),
            
            ["majestic_sword_trinket"] = new(
                Id: "majestic_sword_trinket",
                Name: "Majestic Sword Trinket",
                Description: "Your attacks deal +2 damage. Fits right into the tablet!",
                Price: 75,
                Level: 3,
                Usage: CardUsage.Passive,
                Effect: (state, playerId) =>
                {
                    
                }
            ),
            
            ["armor_of_irrelevance"] = new(
                Id: "armor_of_irrelevance",
                Name: "Armor of Irrelevance",
                Description: "Take 2 less damage from non-villain enemies. Just not important enough",
                Price: 75,
                Level: 3,
                Usage: CardUsage.Passive,
                Effect: (state, playerId) =>
                {
                    
                }
            ),
        };
}