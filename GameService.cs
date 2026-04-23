using WorldSearch.Models;

namespace WorldSearch.Services;

public class GameService
{
    private readonly Dictionary<string, GameSession> _sessions = new();
    private readonly Random _rng = new();

    // ── Session Management ────────────────────────────────────────────

    public GameSession CreateSession(string connectionId, string hostName)
    {
        var code = GenerateRoomCode();
        var host = new Player
        {
            Id = Guid.NewGuid().ToString("N")[..8],
            ConnectionId = connectionId,
            Name = hostName,
            IsHost = true
        };
        var session = new GameSession
        {
            RoomCode = code,
            Players = new List<Player> { host }
        };
        _sessions[code] = session;
        return session;
    }

    public (GameSession? session, Player? player, string? error) JoinSession(
        string roomCode, string connectionId, string playerName)
    {
        roomCode = roomCode.ToUpper();
        if (!_sessions.TryGetValue(roomCode, out var session))
            return (null, null, "Room not found.");

        if (session.State != GameState.Lobby)
            return (null, null, "Game is already running.");

        if (session.Players.Count >= 8)
            return (null, null, "Room is full (max. 8 players).");

        var player = new Player
        {
            Id = Guid.NewGuid().ToString("N")[..8],
            ConnectionId = connectionId,
            Name = playerName,
            IsHost = false
        };
        session.Players.Add(player);
        return (session, player, null);
    }

    public GameSession? GetSessionByConnection(string connectionId)
        => _sessions.Values.FirstOrDefault(s =>
            s.Players.Any(p => p.ConnectionId == connectionId));

    public GameSession? GetSession(string roomCode)
        => _sessions.GetValueOrDefault(roomCode.ToUpper());

    public Player? GetPlayer(GameSession session, string connectionId)
        => session.Players.FirstOrDefault(p => p.ConnectionId == connectionId);

    // ── Game Flow ─────────────────────────────────────────────────────

    public (bool success, string? error) StartGame(GameSession session, string connectionId)
    {
        var player = GetPlayer(session, connectionId);
        if (player is null || !player.IsHost)
            return (false, "Only the host can start the game.");

        if (session.State != GameState.Lobby)
            return (false, "Game is already running.");

        if (session.Players.Count < 1)
            return (false, "At least 1 player required.");

        // Pick random items
        var shuffled = ItemCatalog.AllItems
            .OrderBy(_ => _rng.Next())
            .Take(session.ItemsPerRound)
            .Select(t => new GameItem { Name = t.Name, Emoji = t.Emoji, Category = t.Category })
            .ToList();

        session.ActiveItems = shuffled;
        session.FoundItems.Clear();
        foreach (var p in session.Players) p.FoundItemIds.Clear();

        session.State = GameState.Running;
        session.StartedAt = DateTime.UtcNow;
        session.EndsAt = DateTime.UtcNow.AddMinutes(session.RoundDurationMinutes);
        session.WinnerId = null;

        return (true, null);
    }

    public (bool success, string? error, FoundItem? found) SubmitPhoto(
        GameSession session, string connectionId, string itemId, string photoBase64)
    {
        if (session.State != GameState.Running)
            return (false, "No active game.", null);

        if (session.IsExpired)
        {
            FinishGame(session, expired: true);
            return (false, "Time's up!", null);
        }

        var player = GetPlayer(session, connectionId);
        if (player is null)
            return (false, "Player not found.", null);

        var item = session.ActiveItems.FirstOrDefault(i => i.Id == itemId);
        if (item is null)
            return (false, "Item not found.", null);

        if (player.FoundItemIds.Contains(itemId))
            return (false, "Already found.", null);

        // Validate photo size (max ~5MB base64)
        if (photoBase64.Length > 7_000_000)
            return (false, "Photo too large (max. 5 MB).", null);

        player.FoundItemIds.Add(itemId);

        var foundItem = new FoundItem
        {
            ItemId = itemId,
            PlayerId = player.Id,
            PlayerName = player.Name,
            PhotoBase64 = photoBase64,
            FoundAt = DateTime.UtcNow
        };
        session.FoundItems.Add(foundItem);

        // Check win: player found ALL items
        if (player.FoundItemIds.Count >= session.ActiveItems.Count)
        {
            session.WinnerId = player.Id;
            FinishGame(session, expired: false);
        }

        return (true, null, foundItem);
    }

    public void FinishGame(GameSession session, bool expired)
    {
        session.State = GameState.Finished;
        session.EndsAt = DateTime.UtcNow; // stop the clock

        // If expired and no winner yet, pick player with most finds
        if (session.WinnerId is null)
        {
            var top = session.Players.OrderByDescending(p => p.Score).FirstOrDefault();
            if (top is not null && top.Score > 0)
                session.WinnerId = top.Id;
        }
    }

    public void RemovePlayer(GameSession session, string connectionId)
    {
        var player = session.Players.FirstOrDefault(p => p.ConnectionId == connectionId);
        if (player is null) return;

        session.Players.Remove(player);

        // If host left, assign new host
        if (player.IsHost && session.Players.Count > 0)
            session.Players[0].IsHost = true;

        // Clean up empty sessions
        if (session.Players.Count == 0)
            _sessions.Remove(session.RoomCode);
    }

    public void RestartGame(GameSession session, string connectionId)
    {
        var player = GetPlayer(session, connectionId);
        if (player is null || !player.IsHost) return;

        session.State = GameState.Lobby;
        session.ActiveItems.Clear();
        session.FoundItems.Clear();
        session.WinnerId = null;
        session.StartedAt = null;
        session.EndsAt = null;
        foreach (var p in session.Players) p.FoundItemIds.Clear();
    }

    // ── DTO Mapping ───────────────────────────────────────────────────

    public GameStateDto ToDto(GameSession session) => new(
        session.RoomCode,
        session.State.ToString(),
        session.Players.Select(p => new PlayerDto(
            p.Id, p.Name, p.IsHost, p.Score, p.FoundItemIds)).ToList(),
        session.ActiveItems.Select(i => new GameItemDto(
            i.Id, i.Name, i.Emoji, i.Category)).ToList(),
        session.FoundItems.Select(f => new FoundItemDto(
            f.ItemId, f.PlayerId, f.PlayerName,
            f.PhotoBase64,
            f.FoundAt.ToString("HH:mm:ss"))).ToList(),
        session.SecondsRemaining,
        session.WinnerId
    );

    // ── Helpers ───────────────────────────────────────────────────────

    private string GenerateRoomCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        string code;
        do { code = new string(Enumerable.Range(0, 6).Select(_ => chars[_rng.Next(chars.Length)]).ToArray()); }
        while (_sessions.ContainsKey(code));
        return code;
    }
}
