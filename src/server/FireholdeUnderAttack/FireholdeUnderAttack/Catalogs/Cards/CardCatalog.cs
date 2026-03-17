using System.Collections.ObjectModel;
using FireholdeUnderAttack.Cards;
using FireholdeUnderAttack.Catalogs.Cards;

namespace FireholdeUnderAttack.Constants;

public static class CardCatalog
{
    public static readonly IReadOnlyDictionary<string, CardDefinition> All =
        new Dictionary<string, CardDefinition>()
            .Concat(Level1CardCatalog.Cards)
            .Concat(Level2CardCatalog.Cards)
            .Concat(Level3CardCatalog.Cards)
            .Concat(Level4CardCatalog.Cards)
            .Concat(Level5CardCatalog.Cards)
            .ToDictionary();
}
