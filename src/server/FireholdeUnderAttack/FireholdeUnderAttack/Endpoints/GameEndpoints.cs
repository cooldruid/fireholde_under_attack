using FireholdeUnderAttack.Commands;
using FireholdeUnderAttack.Managers;

namespace FireholdeUnderAttack.Endpoints;

public static class GameEndpoints
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/games/create", (CreateGameRequest request, GameInstanceManager manager) =>
        {
            var game = manager.Create(request.PlayerId, request.PlayerName);
            return TypedResults.Ok(new CreateGameResponse(game.Id, request.PlayerId));
        });

        app.MapPost("/games/join", async (JoinGameRequest request, GameInstanceManager manager) =>
        {
            try
            {
                var game = manager.Get(request.GameId.ToString());
                await game.Enqueue(new JoinGameCommand { PlayerId = request.PlayerId, PlayerName = request.PlayerName });
                return Results.Ok(new JoinGameResponse(game.Id, game.State.Players.Select(x => x.PlayerId).ToList()));
            }
            catch (ArgumentException)
            {
                return Results.NotFound($"Game {request.GameId} not found");
            }
        }).Produces<JoinGameResponse>();

        app.MapGet("/games/{gameId}/state", (Guid gameId, GameInstanceManager manager) =>
        {
            try
            {
                var game = manager.Get(gameId.ToString());
                var s = game.State;
                return Results.Ok(new GameStateResponse(
                    s.GameId,
                    s.OwnerId,
                    s.SequenceNumber,
                    s.State.ToString(),
                    s.TurnMarker?.ActivePlayerId,
                    s.Round,
                    s.Players.Select(p => new PlayerStateResponse(p.PlayerId, p.PlayerName, p.CurrentTile, p.Health)).ToList(),
                    s.Board.Tiles.Select(t => new TileStateResponse(t.Id, t.Type.ToString())).ToList()
                ));
            }
            catch (ArgumentException)
            {
                return Results.NotFound($"Game {gameId} not found");
            }
        }).Produces<GameStateResponse>();

        app.MapPost("/games/{gameId}/commands", async (Guid gameId, ICommand command, GameInstanceManager manager) =>
        {
            try
            {
                var game = manager.Get(gameId.ToString());
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

public record CreateGameRequest(Guid PlayerId, string PlayerName);
public record JoinGameRequest(Guid GameId, Guid PlayerId, string PlayerName);

public record CreateGameResponse(Guid GameId, Guid PlayerId);
public record JoinGameResponse(Guid GameId, List<Guid> PlayerIds);

public record GameStateResponse(
    Guid GameId,
    Guid OwnerId,
    int SequenceNumber,
    string State,
    Guid? ActivePlayerId,
    int Round,
    List<PlayerStateResponse> Players,
    List<TileStateResponse> Board);

public record PlayerStateResponse(Guid PlayerId, string PlayerName, int CurrentTile, int Health);
public record TileStateResponse(int Id, string Type);
