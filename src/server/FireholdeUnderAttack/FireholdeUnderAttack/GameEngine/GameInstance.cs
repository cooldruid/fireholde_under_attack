using System.Text.Json;
using System.Threading.Channels;
using FireholdeUnderAttack.Commands;
using FireholdeUnderAttack.Events;
using FireholdeUnderAttack.Hubs;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace FireholdeUnderAttack.GameEngine;

public class GameInstance
{
    private readonly Channel<ICommand> _channel;
    private readonly IHubContext<EventHub> _hubContext;
    private GameState _state;

    public Guid Id { get; }
    public GameState State => _state;

    public GameInstance(Guid gameOwnerId, string ownerName, IHubContext<EventHub> hubContext)
    {
        Id = Guid.NewGuid();
        _state = GameState.Create(gameOwnerId, ownerName);
        _state.GameId = Id;
        _hubContext = hubContext;

        _channel = Channel.CreateUnbounded<ICommand>();
        _ = ProcessLoop();
    }

    public Task Enqueue(ICommand command)
        => _channel.Writer.WriteAsync(command).AsTask();

    private async Task ProcessLoop()
    {
        await foreach (var command in _channel.Reader.ReadAllAsync())
        {
            try
            {
                var stateMachine = new GameStateMachine(_state);
                var result = stateMachine.Handle(command);
                await BroadcastAsync(result.Events);

                if (result.OnEnterCommand != null)
                    await Enqueue(result.OnEnterCommand);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error processing command {Command} in game {GameId}", command.GetType().Name, Id);
            }
        }
    }

    private async Task BroadcastAsync(List<IEvent> events)
    {
        foreach (var @event in events)
        {
            @event.SequenceNumber = ++_state.SequenceNumber;
            var method = @event.GetType().Name;
            var payload = JsonSerializer.Serialize(@event, @event.GetType());
            await _hubContext.Clients.Group(Id.ToString()).SendAsync(method, payload);
        }
    }
}
