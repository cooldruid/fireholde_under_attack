namespace FireholdeUnderAttack.Events;

public class ShopOpenedEvent : IEvent
{
    public int SequenceNumber { get; set; }
    public Guid GameId { get; init; }
    public Guid PlayerId { get; init; }
    public List<string> AvailableCardIds { get; init; } = [];
}
