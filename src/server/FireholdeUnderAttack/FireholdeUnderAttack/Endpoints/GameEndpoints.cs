using FireholdeUnderAttack.Commands;
using FireholdeUnderAttack.Managers;

namespace FireholdeUnderAttack.Endpoints;

public static class GameEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/games/{gameId}/commands", async (Guid gameId, ICommand command, GameInstanceManager manager) =>
        {
            try
            {
                var game = manager.Get(gameId.ToString());
                command.GameId = gameId;
                await game.Enqueue(command);
                return Results.Accepted();
            }
            catch (ArgumentException)
            {
                return Results.NotFound($"Game {gameId} not found");
            }
        });
    }
}
