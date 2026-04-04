namespace FireholdeUnderAttack.Data;

public class TurnMarker
{
    public Guid? ActivePlayerId { get; set; }
    public int ActivePlayerIndex { get; set; }
    public int ActionsRemaining { get; set; }
}
