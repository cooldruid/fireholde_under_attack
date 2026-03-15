namespace FireholdeUnderAttack.Events;

public interface IEvent
{
    int SequenceNumber { get; set; }
}