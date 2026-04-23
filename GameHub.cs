using Microsoft.AspNetCore.SignalR;
using WorldSearch.Models;
using WorldSearch.Services;

namespace WorldSearch.Hubs;

public class GameHub : Hub
{
    private readonly GameService _gameService;
    private readonly ILogger<GameHub> _logger;

    public GameHub(GameService gameService, ILogger<GameHub> logger)
    {
        _gameService = gameService;
        _logger = logger;
    }

    // ── Lobby ─────────────────────────────────────────────────────────

    /// <summary>Host creates a new room.</summary>
    public async Task CreateRoom(string hostName)
    {
        hostName = hostName.Trim();
        if (string.IsNullOrWhiteSpace(hostName))
        {
            await Clients.Caller.SendAsync("Error", "Name darf nicht leer sein.");
            return;
        }

        var session = _gameService.CreateSession(Context.ConnectionId, hostName);
        await Groups.AddToGroupAsync(Context.ConnectionId, session.RoomCode);
        await Clients.Caller.SendAsync("RoomCreated", session.RoomCode);
        await BroadcastState(session);
        _logger.LogInformation("Room {Code} created by {Name}", session.RoomCode, hostName);
    }

    /// <summary>Player joins an existing room.</summary>
    public async Task JoinRoom(string roomCode, string playerName)
    {
        playerName = playerName.Trim();
        if (string.IsNullOrWhiteSpace(playerName))
        {
            await Clients.Caller.SendAsync("Error", "Name darf nicht leer sein.");
            return;
        }

        var (session, player, error) = _gameService.JoinSession(roomCode, Context.ConnectionId, playerName);
        if (error is not null)
        {
            await Clients.Caller.SendAsync("Error", error);
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, session!.RoomCode);
        await Clients.Caller.SendAsync("JoinedRoom", session.RoomCode, player!.Id);
        await BroadcastState(session);
        _logger.LogInformation("{Name} joined room {Code}", playerName, session.RoomCode);
    }

    // ── Game Control ──────────────────────────────────────────────────

    public async Task StartGame(string roomCode)
    {
        var session = _gameService.GetSession(roomCode);
        if (session is null) { await Clients.Caller.SendAsync("Error", "Raum nicht gefunden."); return; }

        var (success, error) = _gameService.StartGame(session, Context.ConnectionId);
        if (!success) { await Clients.Caller.SendAsync("Error", error); return; }

        await BroadcastState(session);
        _logger.LogInformation("Game started in room {Code}", roomCode);

        // Background timer: end game when time is up
        _ = Task.Run(async () =>
        {
            await Task.Delay(TimeSpan.FromMinutes(session.RoundDurationMinutes));
            if (session.State == GameState.Running)
            {
                _gameService.FinishGame(session, expired: true);
                await BroadcastState(session);
                _logger.LogInformation("Game {Code} ended by timer", roomCode);
            }
        });
    }

    public async Task RestartGame(string roomCode)
    {
        var session = _gameService.GetSession(roomCode);
        if (session is null) return;

        _gameService.RestartGame(session, Context.ConnectionId);
        await BroadcastState(session);
    }

    // ── Photo Submission ──────────────────────────────────────────────

    public async Task SubmitPhoto(string roomCode, string itemId, string photoBase64)
    {
        var session = _gameService.GetSession(roomCode);
        if (session is null) { await Clients.Caller.SendAsync("Error", "Raum nicht gefunden."); return; }

        var (success, error, foundItem) = _gameService.SubmitPhoto(session, Context.ConnectionId, itemId, photoBase64);
        if (!success)
        {
            await Clients.Caller.SendAsync("Error", error);
            return;
        }

        // Broadcast the new found item to all players in room
        await BroadcastState(session);

        // Notify if game ended (win or time)
        if (session.State == GameState.Finished)
        {
            var winner = session.Players.FirstOrDefault(p => p.Id == session.WinnerId);
            await Clients.Group(roomCode).SendAsync("GameOver",
                session.WinnerId,
                winner?.Name ?? "Niemand",
                "win");
        }
    }

    // ── Timer Sync ────────────────────────────────────────────────────

    public async Task GetTimer(string roomCode)
    {
        var session = _gameService.GetSession(roomCode);
        if (session is null) return;

        if (session.State == GameState.Running && session.IsExpired)
        {
            _gameService.FinishGame(session, expired: true);
            await BroadcastState(session);
            await Clients.Group(roomCode).SendAsync("GameOver", null, "Niemand", "timeout");
        }
        else
        {
            await Clients.Caller.SendAsync("TimerUpdate", session.SecondsRemaining);
        }
    }

    // ── Disconnect ────────────────────────────────────────────────────

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var session = _gameService.GetSessionByConnection(Context.ConnectionId);
        if (session is not null)
        {
            var player = _gameService.GetPlayer(session, Context.ConnectionId);
            _gameService.RemovePlayer(session, Context.ConnectionId);

            if (session.Players.Count > 0)
                await BroadcastState(session);
        }
        await base.OnDisconnectedAsync(exception);
    }

    // ── Helpers ───────────────────────────────────────────────────────

    private async Task BroadcastState(GameSession session)
    {
        var dto = _gameService.ToDto(session);
        await Clients.Group(session.RoomCode).SendAsync("StateUpdate", dto);
    }
}
