using System.Text.Json.Serialization;

namespace FireholdeUnderAttack.Commands;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(MoveCommand), nameof(MoveCommand))]
[JsonDerivedType(typeof(StartGameCommand), nameof(StartGameCommand))]
[JsonDerivedType(typeof(JoinGameCommand), nameof(JoinGameCommand))]
[JsonDerivedType(typeof(BuyCardCommand), nameof(BuyCardCommand))]
[JsonDerivedType(typeof(DoneShoppingCommand), nameof(DoneShoppingCommand))]
[JsonDerivedType(typeof(UseCardCommand), nameof(UseCardCommand))]
public interface ICommand { }