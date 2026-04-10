namespace FireholdeUnderAttack.Events;

public class VillainDefeatedEvent : IEvent
{
    public int SequenceNumber { get; set; }
    public Guid GameId { get; init; }
    public Guid KilledByPlayerId { get; init; }
}
