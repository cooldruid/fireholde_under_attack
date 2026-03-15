namespace FireholdeUnderAttack.Events;

public class TurnChangedEvent : IEvent
{
    public int SequenceNumber { get; set; }
    public Guid GameId { get; init; }
    public Guid? ActivePlayerId { get; init; }
    public bool IsVillainTurn { get; init; }
    public int Round { get; init; }
}
