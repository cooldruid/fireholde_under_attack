namespace FireholdeUnderAttack.Data;

public class Enemy
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public int Health { get; set; }
    public int Position { get; set; }

    private Enemy() { }

    public static Enemy Create(Board board, string name = "Enemy") =>
        new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Health = 10,
            Position = Random.Shared.Next(board.Tiles.Count)
        };
}
