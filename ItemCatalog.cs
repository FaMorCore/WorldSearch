namespace WorldSearch.Services;

public static class ItemCatalog
{
    /// <summary>
    /// Hier kannst du deine eigene Liste an Such-Gegenständen pflegen!
    /// Einfach neue GameItems hinzufügen oder entfernen.
    /// </summary>
    public static readonly List<(string Name, string Emoji, string Category)> AllItems = new()
    {
        // 🌿 Natur
        ("Ein roter Briefkasten",     "📮", "Straße"),
        ("Eine Katze",                "🐱", "Tiere"),
        ("Ein Hund an der Leine",     "🐕", "Tiere"),
        ("Ein Fahrrad",               "🚲", "Fahrzeuge"),
        ("Eine Blume",                "🌸", "Natur"),
        ("Ein Baum mit roter Rinde",  "🌲", "Natur"),
        ("Ein Pilz",                  "🍄", "Natur"),
        ("Eine Pfütze",               "💧", "Natur"),
        ("Ein Vogel",                 "🐦", "Tiere"),
        ("Ein Schmetterling",         "🦋", "Tiere"),
        ("Ein Stein mit Moos",        "🪨", "Natur"),
        ("Ein umgefallener Baum",     "🪵", "Natur"),

        // 🏙️ Stadt
        ("Ein Straßenschild",         "🪧", "Stadt"),
        ("Eine Ampel",                "🚦", "Stadt"),
        ("Ein Mülleimer",             "🗑️", "Stadt"),
        ("Ein Laternenpfahl",         "💡", "Stadt"),
        ("Eine Parkbank",             "🪑", "Stadt"),
        ("Ein Brunnen",               "⛲", "Stadt"),
        ("Ein Graffiti",              "🎨", "Stadt"),
        ("Ein Hydranten",             "🚒", "Stadt"),
        ("Eine Baustelle",            "🏗️", "Stadt"),
        ("Ein Supermarkt",            "🏪", "Stadt"),
        ("Ein Café",                  "☕", "Stadt"),
        ("Ein Kiosk",                 "🏬", "Stadt"),
        ("Eine Kirche",               "⛪", "Stadt"),
        ("Ein Denkmal",               "🗽", "Stadt"),

        // 🌤️ Wetter & Himmel
        ("Eine Wolke",                "☁️", "Himmel"),
        ("Einen Kondensstreifen",     "✈️", "Himmel"),
        ("Den Sonnenuntergang",       "🌅", "Himmel"),
        ("Einen Regenbogen",          "🌈", "Himmel"),

        // 🍕 Essen & Trinken
        ("Eine leere Flasche",        "🍾", "Fundstücke"),
        ("Eine weggeworfene Dose",    "🥫", "Fundstücke"),
        ("Eine Eiswaffel",            "🍦", "Essen"),
        ("Ein Eis in deiner Hand",    "🍨", "Essen"),

        // 🎭 Menschen & Aktivitäten
        ("Jemanden mit Hut",          "🎩", "Menschen"),
        ("Jemanden auf einem Skateboard", "🛹", "Menschen"),
        ("Jemanden der Musik hört",   "🎧", "Menschen"),
        ("Zwei Menschen die lachen",  "😄", "Menschen"),
        ("Ein Kind das spielt",       "🧒", "Menschen"),

        // 🎲 Spezial-Challenges
        ("Dein eigener Schatten",     "👥", "Kreativ"),
        ("Etwas Blaues und Rundes",   "🔵", "Kreativ"),
        ("Etwas komplett Weißes",     "⬜", "Kreativ"),
        ("Eine Zahl größer als 100",  "🔢", "Kreativ"),
        ("Drei gleiche Gegenstände",  "🔄", "Kreativ"),
        ("Etwas sehr Altes",          "⏳", "Kreativ"),
        ("Etwas sehr Neues",          "✨", "Kreativ"),
    };
}
