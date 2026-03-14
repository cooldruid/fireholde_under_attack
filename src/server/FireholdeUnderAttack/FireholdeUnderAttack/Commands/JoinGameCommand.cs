namespace FireholdeUnderAttack.Commands;

public class JoinGameCommand : ICommand
{
    public Guid GameId { get; set; }
    public Guid PlayerId { get; set; }
}
