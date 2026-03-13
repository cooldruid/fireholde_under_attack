namespace FireholdeUnderAttack.Commands;

public class MoveCommand : ICommand
{
    public Guid GameId { get; set; }
    public Guid PlayerId { get; set; }
}