# 🌍 WorldSearch

Ein **GeoBingo-inspiriertes Multiplayer-Browserspiel** für dich und deine Freunde.
Gebaut mit **ASP.NET Core 8** + **SignalR** (Echtzeit-Updates).

## 🎮 Spielprinzip

- **Host** erstellt einen Raum → bekommt einen **6-stelligen Raum-Code**
- Freunde joinen mit dem Code (max. 8 Spieler)
- Beim Start werden **8 zufällige Gegenstände** aus deiner Liste gezogen
- Jeder Spieler hat **10 Minuten** Zeit, alle zu finden & zu fotografieren
- **Gewinner**: Wer zuerst alle 8 hat — oder bei Zeitablauf der mit den meisten Funden

## 🚀 Starten

### Voraussetzungen
- [.NET 8 SDK](https://dotnet.microsoft.com/download)

### Lokal ausführen

```bash
cd WorldSearch
dotnet run
```

Dann im Browser öffnen: `http://localhost:5000` (Port wird beim Start angezeigt).

### Im LAN spielen (mit Freunden im selben WLAN)

```bash
dotnet run --urls "http://0.0.0.0:5000"
```

Deine lokale IP rausfinden (Windows: `ipconfig`, Mac/Linux: `ifconfig`),
dann teilen deine Freunde z.B. `http://192.168.1.42:5000`.

### Übers Internet spielen
Am einfachsten mit **ngrok** oder **Cloudflare Tunnel**:
```bash
ngrok http 5000
```

## 🛠️ Anpassen

### Eigene Gegenstände hinzufügen

Öffne `Services/ItemCatalog.cs` und ergänze die `AllItems`-Liste:

```csharp
("Dein Gegenstand",  "🎯", "Kategorie"),
```

### Spieleinstellungen ändern
In `Models/Models.cs` die `GameSession`-Defaults:

```csharp
public int RoundDurationMinutes { get; set; } = 10;  // Rundenzeit
public int ItemsPerRound { get; set; } = 8;          // Anzahl Items pro Runde
```

## 📁 Projekt-Struktur

```
WorldSearch/
├── Program.cs              ← Entry-Point, DI, Routing
├── WorldSearch.csproj
├── Hubs/
│   └── GameHub.cs          ← SignalR-Endpunkte (CreateRoom, JoinRoom, SubmitPhoto…)
├── Services/
│   ├── GameService.cs      ← Spiel-Logik (Sessions, Rundenstart, Gewinn-Check)
│   └── ItemCatalog.cs      ← ⭐ Hier deine Such-Liste anpassen!
├── Models/
│   └── Models.cs           ← Datenmodelle + DTOs
└── wwwroot/
    └── index.html          ← Komplettes Frontend (HTML+CSS+JS in einer Datei)
```

## 🔌 SignalR Events (für eigene Erweiterungen)

| Client → Server   | Beschreibung                  |
|-------------------|-------------------------------|
| `CreateRoom`      | Host erstellt Raum            |
| `JoinRoom`        | Spieler tritt bei             |
| `StartGame`       | Host startet Runde            |
| `SubmitPhoto`     | Spieler reicht Foto ein       |
| `RestartGame`     | Host startet neue Runde       |
| `GetTimer`        | Timer-Sync vom Server         |

| Server → Client   | Beschreibung                  |
|-------------------|-------------------------------|
| `RoomCreated`     | Raum-Code zurück an Host      |
| `JoinedRoom`      | Bestätigung an Spieler        |
| `StateUpdate`     | Vollständiger Spielzustand    |
| `TimerUpdate`     | Sekunden-Resync               |
| `GameOver`        | Gewinner / Zeitablauf         |
| `Error`           | Fehlermeldung                 |

## 💡 Mögliche Erweiterungen

- 📊 Highscore-Persistenz (SQLite)
- 🗳️ Voting-System: Mitspieler bestätigen Foto
- 📍 GPS-Verifikation (HTML5 Geolocation)
- 🎨 Eigene Item-Listen pro Raum hochladen
- 🤖 KI-Bilderkennung (Azure Vision / OpenAI Vision API) zur automatischen Validierung

## ⚙️ Tech-Notes

- **Sessions im Memory** (kein DB nötig für Friends-Only)
- **Fotos als Base64** über SignalR (max. 5 MB / Foto, 10 MB Hub-Limit)
- **Singleton GameService** hält alle aktiven Räume
- Bei Disconnect wird Spieler entfernt; Host-Rolle wandert automatisch weiter

Viel Spaß! 🎉
