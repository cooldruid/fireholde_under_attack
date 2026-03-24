namespace FireholdeUnderAttack.Data;

public class Villain
{
    public int MaxHealth { get; init; }
    public int CurrentHealth { get; init; }
    
    private Villain()
    { }

    public static Villain Create(int maxHealth) =>
        new()
        {
            MaxHealth = maxHealth,
            CurrentHealth = maxHealth
        };
}