using FireholdeUnderAttack.GameEngine;

namespace FireholdeUnderAttack.Catalogs.Classes;

public record ClassFeatureDefinition(string Name, string Description, ClassFeatureEffect Effect);
public delegate void ClassFeatureEffect(GameState state, Guid playerId);

public record ClassDefinition(
    string Id,
    string Name,
    string Description,
    ClassFeatureDefinition Level2Feature,
    ClassFeatureDefinition Level3Feature,
    ClassFeatureDefinition Level4Feature
);
