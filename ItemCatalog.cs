namespace WorldSearch.Services;

public static class ItemCatalog
{
    /// <summary>
    /// Master list of all possible search items.
    /// Add or remove entries to customize your game!
    /// 8 random items will be picked from this list each round.
    /// </summary>
    public static readonly List<(string Name, string Emoji, string Category)> AllItems = new()
    {
        // 🌿 Nature
        ("A red mailbox",                 "📮", "Street"),
        ("A cat",                         "🐱", "Animals"),
        ("A dog on a leash",              "🐕", "Animals"),
        ("A bicycle",                     "🚲", "Vehicles"),
        ("A flower",                      "🌸", "Nature"),
        ("A tree with red bark",          "🌲", "Nature"),
        ("A mushroom",                    "🍄", "Nature"),
        ("A puddle",                      "💧", "Nature"),
        ("A bird",                        "🐦", "Animals"),
        ("A butterfly",                   "🦋", "Animals"),
        ("A mossy stone",                 "🪨", "Nature"),
        ("A fallen tree",                 "🪵", "Nature"),

        // 🏙️ City
        ("A street sign",                 "🪧", "City"),
        ("A traffic light",               "🚦", "City"),
        ("A trash can",                   "🗑️", "City"),
        ("A lamp post",                   "💡", "City"),
        ("A park bench",                  "🪑", "City"),
        ("A fountain",                    "⛲", "City"),
        ("A graffiti",                    "🎨", "City"),
        ("A fire hydrant",                "🚒", "City"),
        ("A construction site",           "🏗️", "City"),
        ("A supermarket",                 "🏪", "City"),
        ("A café",                        "☕", "City"),
        ("A kiosk",                       "🏬", "City"),
        ("A church",                      "⛪", "City"),
        ("A monument",                    "🗽", "City"),

        // 🌤️ Sky & Weather
        ("A cloud",                       "☁️", "Sky"),
        ("A contrail / plane trail",      "✈️", "Sky"),
        ("A sunset",                      "🌅", "Sky"),
        ("A rainbow",                     "🌈", "Sky"),

        // 🍕 Food & Drink
        ("An empty bottle",               "🍾", "Found Items"),
        ("A discarded can",               "🥫", "Found Items"),
        ("An ice cream cone",             "🍦", "Food"),
        ("Ice cream in your hand",        "🍨", "Food"),

        // 🎭 People & Activities
        ("Someone wearing a hat",         "🎩", "People"),
        ("Someone on a skateboard",       "🛹", "People"),
        ("Someone listening to music",    "🎧", "People"),
        ("Two people laughing",           "😄", "People"),
        ("A child playing",               "🧒", "People"),

        // 🎲 Creative Challenges
        ("Your own shadow",               "👥", "Creative"),
        ("Something blue and round",      "🔵", "Creative"),
        ("Something completely white",    "⬜", "Creative"),
        ("A number greater than 100",     "🔢", "Creative"),
        ("Three identical objects",       "🔄", "Creative"),
        ("Something very old",            "⏳", "Creative"),
        ("Something very new",            "✨", "Creative"),
    };
}
