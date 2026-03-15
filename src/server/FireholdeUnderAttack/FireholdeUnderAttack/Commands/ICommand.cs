using System.Text.Json.Serialization;

namespace FireholdeUnderAttack.Commands;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(MoveCommand), nameof(MoveCommand))]
[JsonDerivedType(typeof(StartGameCommand), nameof(StartGameCommand))]
[JsonDerivedType(typeof(JoinGameCommand), nameof(JoinGameCommand))]
public interface ICommand { }