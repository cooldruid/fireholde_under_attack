namespace FireholdeUnderAttack.Data;

public class Villain
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }

    private Villain() { }

    public static Villain Create(int playerCount) =>
        new()
        {
            Id = Guid.NewGuid(),
            Name = "Villain",
            MaxHealth = GameConfig.BossHealthPerPlayer * playerCount,
            CurrentHealth = GameConfig.BossHealthPerPlayer * playerCount
        };
}