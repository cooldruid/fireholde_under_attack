namespace FireholdeUnderAttack.Commands;

public class MoveCommand : ICommand
{
    public Guid PlayerId { get; set; }
}