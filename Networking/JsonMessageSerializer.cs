using System.Text.Json;

namespace Rpg;

public sealed class JsonMessageSerializer
{
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    public string Serialize(NetworkMessageDto message)
    {
        return JsonSerializer.Serialize(message, _options);
    }

    public NetworkMessageDto? Deserialize(string json)
    {
        return JsonSerializer.Deserialize<NetworkMessageDto>(json, _options);
    }
}