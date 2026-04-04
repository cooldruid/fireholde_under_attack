namespace FireholdeUnderAttack.Events;

public class GameStartedEvent : IEvent
{
    public int SequenceNumber { get; set; }
    public Guid GameId { get; init; }
    public Guid? ActivePlayerId { get; init; }
    public int Round { get; init; }
}
