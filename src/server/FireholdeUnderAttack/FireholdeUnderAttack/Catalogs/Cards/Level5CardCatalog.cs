using FireholdeUnderAttack.Cards;

namespace FireholdeUnderAttack.Catalogs.Cards;

public class Level5CardCatalog
{
    public static readonly IReadOnlyDictionary<string, CardDefinition> Cards =
        new Dictionary<string, CardDefinition>
        {
            // Level 1
            ["healing_potion"] = new(
                Id: "healing_potion",
                Name: "Healing Potion",
                Description: "Restore 10 health.",
                Price: 20,
                Level: 1,
                Usage: CardUsage.Active,
                Effect: (state, playerId) =>
                {
                    var player = state.Players.First(p => p.PlayerId == playerId);
                    player.Health += 10;
                }
            ),

            // Level 2
            // ...

            // Level 3
            // ...
        };
}