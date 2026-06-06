using System.Net.Sockets;

namespace Rpg;

public sealed class ClientConnection
{
    private readonly TcpClient _tcpClient;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;
    private readonly JsonMessageSerializer _serializer = new();

    public int PlayerId { get; }
    public string PlayerName { get; }

    public ClientConnection(TcpClient tcpClient, int playerId, string playerName)
    {
        _tcpClient = tcpClient;
        PlayerId = playerId;
        PlayerName = playerName;

        NetworkStream stream = _tcpClient.GetStream();
        _reader = new StreamReader(stream);
        _writer = new StreamWriter(stream)
        {
            AutoFlush = true
        };
    }

    public async Task<NetworkMessageDto?> ReadMessageAsync()
    {
        string? line = await _reader.ReadLineAsync();

        if (line == null)
            return null;

        return _serializer.Deserialize(line);
    }

    public async Task SendUpdateAsync(GameSnapshotDto snapshot)
    {
        var message = new NetworkMessageDto
        {
            Type = NetworkProtocol.Update,
            Update = new GameUpdateDto
            {
                LocalPlayerId = PlayerId,
                Snapshot = snapshot
            }
        };

        await SendMessageAsync(message);
    }

    public async Task SendErrorAsync(string text)
    {
        var message = new NetworkMessageDto
        {
            Type = NetworkProtocol.Error,
            ErrorMessage = text
        };

        await SendMessageAsync(message);
    }

    private async Task SendMessageAsync(NetworkMessageDto message)
    {
        string json = _serializer.Serialize(message);
        await _writer.WriteLineAsync(json);
    }

    public void Close()
    {
        _tcpClient.Close();
    }
}