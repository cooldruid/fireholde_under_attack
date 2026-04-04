namespace FireholdeUnderAttack.Commands;

public class DoneShoppingCommand : ICommand
{
    public Guid PlayerId { get; init; }
}
