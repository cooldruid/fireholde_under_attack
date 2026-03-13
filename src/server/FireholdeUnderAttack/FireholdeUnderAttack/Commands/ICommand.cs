using System.Text.Json.Serialization;

namespace FireholdeUnderAttack.Commands;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(MoveCommand), nameof(MoveCommand))]
public interface ICommand
{
    public Guid GameId { get; set; }
    public Guid PlayerId { get; set; }
}