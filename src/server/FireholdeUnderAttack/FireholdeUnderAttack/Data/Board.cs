namespace FireholdeUnderAttack.Data;

public class Board
{
    public List<Tile> Tiles { get; init; } = [];
    
    private Board()
    { }

    public static Board Create(List<Tile> tiles) =>
        new()
        {
            Tiles = tiles
        };
}

public class Tile
{
    public int Id { get; set; }
    public BoardTileType Type { get; set; }
}

public enum BoardTileType
{
    Start,
    Shop,
    Quest,
    Orb,
    Empty,
    Graveyard
}