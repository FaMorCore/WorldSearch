namespace WorldSearch.Models;

public class GameItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public string Name { get; set; } = "";
    public string Emoji { get; set; } = "📷";
    public string Category { get; set; } = "Allgemein";
}

public class FoundItem
{
    public string ItemId { get; set; } = "";
    public string PlayerId { get; set; } = "";
    public string PlayerName { get; set; } = "";
    public string PhotoBase64 { get; set; } = "";
    public DateTime FoundAt { get; set; } = DateTime.UtcNow;
}

public class Player
{
    public string Id { get; set; } = "";
    public string ConnectionId { get; set; } = "";
    public string Name { get; set; } = "";
    public bool IsHost { get; set; } = false;
    public List<string> FoundItemIds { get; set; } = new();
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public int Score => FoundItemIds.Count;
}

public class GameSession
{
    public string RoomCode { get; set; } = "";
    public GameState State { get; set; } = GameState.Lobby;
    public List<Player> Players { get; set; } = new();
    public List<GameItem> ActiveItems { get; set; } = new();  // 8 randomly selected
    public List<FoundItem> FoundItems { get; set; } = new();
    public DateTime? StartedAt { get; set; }
    public DateTime? EndsAt { get; set; }
    public int RoundDurationMinutes { get; set; } = 10;
    public int ItemsPerRound { get; set; } = 8;
    public string? WinnerId { get; set; }

    public int SecondsRemaining =>
        EndsAt.HasValue ? Math.Max(0, (int)(EndsAt.Value - DateTime.UtcNow).TotalSeconds) : 0;

    public bool IsExpired =>
        EndsAt.HasValue && DateTime.UtcNow >= EndsAt.Value;
}

public enum GameState
{
    Lobby,
    Running,
    Finished
}

// DTO for client communication
public record PlayerDto(string Id, string Name, bool IsHost, int Score, List<string> FoundItemIds);
public record GameItemDto(string Id, string Name, string Emoji, string Category);
public record FoundItemDto(string ItemId, string PlayerId, string PlayerName, string PhotoBase64, string FoundAt);
public record GameStateDto(
    string RoomCode,
    string State,
    List<PlayerDto> Players,
    List<GameItemDto> ActiveItems,
    List<FoundItemDto> FoundItems,
    int SecondsRemaining,
    string? WinnerId
);
