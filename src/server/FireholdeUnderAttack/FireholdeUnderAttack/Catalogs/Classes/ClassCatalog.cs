using FireholdeUnderAttack.Catalogs.Classes;

namespace FireholdeUnderAttack.Constants;

public static class ClassCatalog
{
    public static readonly IReadOnlyDictionary<string, ClassDefinition> All =
        new Dictionary<string, ClassDefinition>
        {
            ["protector"] = new(
                Id: "protector",
                Name: "Protector",
                Description: "The protector selflessly protects the ones he feels need to be protected. (Speak simple with him, he doesn't know that many words)",
                Level2Feature: new(
                    "Armor up!", 
                    "Take 2 less damage from all sources.", 
                    (gameState, playerId) => { }),
                Level3Feature: new(
                    "Protect",
                    "Gain the Protect action: Choose an ally, the next time they would take damage, you take it instead. Mitigations apply.",
                    (gameState, playerId) => { }),
                Level4Feature: new(
                    "Nuh Uh!",
                    "Gain the Invincible action: Become immune to all damage for 1 turn.",
                    (gameState, playerId) => { })
            ),

            ["elementalist"] = new(
                Id: "elementalist",
                Name: "Elementalist",
                Description: "The elementalist hungers for power and is a jerk about damage statistics.",
                Level2Feature: new(
                    "Elemental Souls", 
                    "Elemental souls spawn on random tiles. Each soul collected permanently adds +1 damage to the Attack action.", 
                    (gameState, playerId) => { }),
                Level3Feature: new(
                    "Double Trouble",
                    "Your Attack action can target two enemies.",
                    (gameState, playerId) => { }),
                Level4Feature: new(
                    "CATACLYSM!!!!",
                    "Gain the Cataclysm action: Attack EVERYONE ELSE 5 times. Oops?",
                    (gameState, playerId) => { })
            ),
            
            ["healer"] = new(
                Id: "healer",
                Name: "Healer",
                Description: "Drawing holy powers, the healer takes care of their allies. (I know, someone has to do it)",
                Level2Feature: new(
                    "Heal", 
                    "Gain the Heal action: Restore 5 HP to a targeted ally.", 
                    (gameState, playerId) => { }),
                Level3Feature: new(
                    "Revive",
                    "While on a Graveyard tile, the healer can spend 100 gold to resurrect a fallen ally.",
                    (gameState, playerId) => { }),
                Level4Feature: new(
                    "Oh my God, oh my God, oh my God",
                    "Gain the Desperate Prayer action: Once per game, revive all allies and restore all their HP. Usable while dead.",
                    (gameState, playerId) => { })
            ),
        };
}
