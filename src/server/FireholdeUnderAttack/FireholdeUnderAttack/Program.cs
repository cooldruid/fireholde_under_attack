using FireholdeUnderAttack.Endpoints;
using FireholdeUnderAttack.Hubs;
using FireholdeUnderAttack.Managers;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Services.AddOpenApi();
builder.Services.AddSignalR();
builder.Services.AddSingleton<GameInstanceManager>();
builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapOpenApi();
app.MapScalarApiReference();

if (app.Environment.IsDevelopment())
    app.UseHttpsRedirection();
app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(_ => true).AllowCredentials());

app.MapHub<EventHub>("/hub");

GameEndpoints.Map(app);

app.Run();