using FireholdeUnderAttack.Cards;

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
                Usage: CardUsage.Active,
                Effect: (state, playerId, targetId) =>
                {
                    
                }
            ),
            
            ["punch_of_midas"] = new(
                Id: "punch_of_midas",
                Name: "Punch Of Midas",
                Description: "Spend all your Gold, deal that much damage",
                Price: 110,
                Level: 4,
                Usage: CardUsage.Active,
                TargetType: CardTargetType.Enemy,
                Effect: (state, playerId, targetId) =>
                {
                    
                }
            ),
            
            ["heart_shaped_crown"] = new(
                Id: "heart_shaped_crown",
                Name: "Heart-Shaped Crown",
                Description: "Increase your current and maximum health by 30",
                Price: 110,
                Level: 4,
                Usage: CardUsage.Passive,
                Effect: (state, playerId, targetId) =>
                {
                    
                }
            ),
            
            ["majestic_shield_trinket"] = new(
                Id: "majestic_shield_trinket",
                Name: "Majestic Shield Trinket",
                Description: "You take 2 less damage from all sources. Fits right into the tablet!",
                Price: 110,
                Level: 4,
                Usage: CardUsage.Passive,
                Effect: (state, playerId, targetId) =>
                {
                    
                }
            ),
            
            ["good_juju_totem"] = new(
                Id: "good_juju_totem",
                Name: "Good Juju Totem",
                Description: "The next time you die, raise again with 15 health",
                Price: 110,
                Level: 4,
                Usage: CardUsage.Passive,
                Effect: (state, playerId, targetId) =>
                {
                    
                }
            ),
        };
}