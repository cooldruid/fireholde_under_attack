namespace FireholdeUnderAttack.Events;

public class CardAcquiredEvent : IEvent
{
    public int SequenceNumber { get; set; }
    public Guid GameId { get; init; }
    public Guid PlayerId { get; init; }
    public string CardId { get; init; } = "";
}
