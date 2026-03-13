namespace FireholdeUnderAttack.Events;

public class MoveEvent : IEvent
{
    public Guid PlayerId { get; set; }
    public int DiceRoll { get; set; }
    public int NewTileId { get; set; }
}