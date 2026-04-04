namespace FireholdeUnderAttack.Events;

public class ShopOpenedEvent : IEvent
{
    public int SequenceNumber { get; set; }
    public Guid GameId { get; init; }
    public Guid PlayerId { get; init; }
    public List<ShopCardInfo> AvailableCards { get; init; } = [];
}

public record ShopCardInfo(string Id, string Name, string Description, int Price, int Level);
