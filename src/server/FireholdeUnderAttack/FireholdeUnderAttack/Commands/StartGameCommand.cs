namespace FireholdeUnderAttack.Commands;

public class StartGameCommand : ICommand
{
    public Guid GameId { get; set; }
    public Guid PlayerId { get; set; }
}
