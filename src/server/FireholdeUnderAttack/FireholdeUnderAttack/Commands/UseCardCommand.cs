namespace FireholdeUnderAttack.Commands;

public class UseCardCommand : ICommand
{
    public Guid PlayerId { get; init; }
    public string CardId { get; init; } = "";
    public Guid? TargetId { get; init; }
}
