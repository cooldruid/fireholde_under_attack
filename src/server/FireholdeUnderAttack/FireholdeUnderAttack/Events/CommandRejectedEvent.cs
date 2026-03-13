namespace FireholdeUnderAttack.Events;

public class CommandRejectedEvent : IEvent
{
    public Guid GameId { get; init; }
    public string Reason { get; init; } = "";
}
