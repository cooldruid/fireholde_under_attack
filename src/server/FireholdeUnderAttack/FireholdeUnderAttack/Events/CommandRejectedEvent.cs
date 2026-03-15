namespace FireholdeUnderAttack.Events;

public class CommandRejectedEvent : IEvent
{
    public int SequenceNumber { get; set; }
    public Guid GameId { get; init; }
    public string Reason { get; init; } = "";
}
