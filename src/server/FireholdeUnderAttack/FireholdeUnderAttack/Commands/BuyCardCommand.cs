namespace FireholdeUnderAttack.Commands;

public class BuyCardCommand : ICommand
{
    public Guid PlayerId { get; init; }
    public string CardId { get; init; } = "";
}
