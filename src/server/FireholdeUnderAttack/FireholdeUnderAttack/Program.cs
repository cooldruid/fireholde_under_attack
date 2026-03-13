using FireholdeUnderAttack.Endpoints;
using FireholdeUnderAttack.Hubs;
using FireholdeUnderAttack.Managers;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Services.AddOpenApi();
builder.Services.AddSignalR();
builder.Services.AddSingleton<GameInstanceManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapHub<EventHub>("/hub");

GameEndpoints.Map(app);

app.Run();