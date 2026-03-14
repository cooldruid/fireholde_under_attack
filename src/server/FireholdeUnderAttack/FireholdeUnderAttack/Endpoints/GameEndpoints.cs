using FireholdeUnderAttack.Commands;
using FireholdeUnderAttack.Managers;

namespace FireholdeUnderAttack.Endpoints;

public static class GameEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/games/create", (CreateGameRequest request, GameInstanceManager manager) =>
        {
            var game = manager.Create(request.PlayerId);
            return TypedResults.Ok(new CreateGameResponse(game.Id, request.PlayerId));
        });

        app.MapPost("/games/join", async (JoinGameRequest request, GameInstanceManager manager) =>
        {
            try
            {
                var game = manager.Get(request.GameId.ToString());
                await game.Enqueue(new JoinGameCommand { GameId = request.GameId, PlayerId = request.PlayerId });
                return Results.Ok(new JoinGameResponse(game.Id, game.State.Players.Select(x => x.Id).ToList()));
            }
            catch (ArgumentException)
            {
                return Results.NotFound($"Game {request.GameId} not found");
            }
        }).Produces<JoinGameResponse>();

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

public record CreateGameRequest(Guid PlayerId);
public record JoinGameRequest(Guid GameId, Guid PlayerId);

public record CreateGameResponse(Guid GameId, Guid PlayerId);
public record JoinGameResponse(Guid GameId, List<Guid> PlayerIds);
