namespace FireholdeUnderAttack.Events;

public class PlayerJoinedEvent : IEvent
{
    public Guid GameId { get; init; }
    public Guid PlayerId { get; init; }
}
