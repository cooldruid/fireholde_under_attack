namespace FireholdeUnderAttack.Commands;

/// <summary>
/// Internal command injected automatically by GameInstance when the villain's turn begins.
/// Never sent via HTTP — not registered as a JsonDerivedType on ICommand.
/// </summary>
public class VillainTurnCommand : ICommand { }
