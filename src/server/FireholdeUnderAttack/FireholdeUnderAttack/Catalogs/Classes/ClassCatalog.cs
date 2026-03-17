using FireholdeUnderAttack.Classes;

namespace FireholdeUnderAttack.Constants;

public static class ClassCatalog
{
    public static readonly IReadOnlyDictionary<string, ClassDefinition> All =
        new Dictionary<string, ClassDefinition>
        {
            ["warrior"] = new(
                Id: "warrior",
                Name: "Warrior",
                Description: "A tough fighter who excels in close combat."
            ),

            ["mage"] = new(
                Id: "mage",
                Name: "Mage",
                Description: "A powerful spellcaster who manipulates magical forces."
            ),
        };
}
