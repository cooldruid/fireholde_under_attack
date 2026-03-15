namespace FireholdeUnderAttack.Events;

public class PlayerJoinedEvent : IEvent
{
    public int SequenceNumber { get; set; }
    public Guid GameId { get; init; }
    public Guid PlayerId { get; init; }
    public string PlayerName { get; init; } = "";
}
