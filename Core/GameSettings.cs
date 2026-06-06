using System.Text.Json;

namespace Rpg;

public sealed class GameSettings
{
    public string PlayerName { get; set; } = "Player";
    public string LogDirectory { get; set; } = "logs";
    public string Theme { get; set; } = "library";

    public static GameSettings Load(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Config file not found: {path}");

        string json = File.ReadAllText(path);

        GameSettings? settings = JsonSerializer.Deserialize<GameSettings>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
        );

        if (settings == null)
            throw new InvalidOperationException("Invalid config file.");

        return settings;
    }
}