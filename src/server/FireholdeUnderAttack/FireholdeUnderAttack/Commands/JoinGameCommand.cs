namespace FireholdeUnderAttack.Commands;

public class JoinGameCommand : ICommand
{
    public Guid PlayerId { get; set; }
    public string PlayerName { get; set; } = "";
}
