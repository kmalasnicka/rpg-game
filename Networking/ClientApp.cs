using System.Net.Sockets;

namespace Rpg;

public sealed class ClientApp
{
    private readonly string _host;
    private readonly int _port;
    private readonly string _playerName;

    private readonly JsonMessageSerializer _serializer = new();
    private readonly ConsoleGameView _view = new();
    private readonly ConsoleInputController _inputController = new();

    private StreamReader? _reader;
    private StreamWriter? _writer;

    private readonly object _snapshotLock = new();

    private GameSnapshotDto? _lastSnapshot;
    private int _localPlayerId;
    private bool _running = true;

    public ClientApp(string host, int port, string playerName)
    {
        _host = host;
        _port = port;
        _playerName = playerName;
    }

    public async Task StartAsync()
    {
        var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync(_host, _port);

        NetworkStream stream = tcpClient.GetStream();

        _reader = new StreamReader(stream);
        _writer = new StreamWriter(stream)
        {
            AutoFlush = true
        };

        await SendLoginAsync();

        _view.Initialize();

        Task receiveTask = Task.Run(ReceiveLoopAsync);

        while (_running)
        {
            if (Console.KeyAvailable)
            {
                ConsoleKey key = Console.ReadKey(intercept: true).Key;

                GameSnapshotDto? snapshot = GetSnapshot();

                if (snapshot == null)
                    continue;

                PlayerActionDto? action = _inputController.ReadAction(
                    key,
                    snapshot,
                    _localPlayerId
                );

                if (action == null)
                    continue;

                if (action.ActionName == "Quit")
                {
                    var disconnectMessage = new NetworkMessageDto
                    {
                        Type = NetworkProtocol.Action,
                        Action = new PlayerActionDto
                        {
                            PlayerId = _localPlayerId,
                            ActionName = "Disconnect"
                        }
                    };

                    await SendMessageAsync(disconnectMessage);

                    _running = false;
                    break;
                }

                action.PlayerId = _localPlayerId;

                var message = new NetworkMessageDto
                {
                    Type = NetworkProtocol.Action,
                    Action = action
                };

                await SendMessageAsync(message);
            }

            await Task.Delay(20);
        }

        tcpClient.Close();
        _view.Shutdown();

        try
        {
            await receiveTask;
        }
        catch
        {
        }
    }

    private async Task SendLoginAsync()
    {
        var message = new NetworkMessageDto
        {
            Type = NetworkProtocol.Login,
            Login = new LoginDto
            {
                PlayerName = _playerName
            }
        };

        await SendMessageAsync(message);
    }

    private async Task ReceiveLoopAsync()
    {
        if (_reader == null)
            return;

        while (_running)
        {
            string? line;

            try
            {
                line = await _reader.ReadLineAsync();
            }
            catch
            {
                return;
            }

            if (line == null)
                return;

            NetworkMessageDto? message = _serializer.Deserialize(line);

            if (message == null)
                continue;

            if (message.Type == NetworkProtocol.Update && message.Update != null)
            {
                GameSnapshotDto snapshot = message.Update.Snapshot;
                int localPlayerId = message.Update.LocalPlayerId;

                lock (_snapshotLock)
                {
                    _lastSnapshot = snapshot;
                    _localPlayerId = localPlayerId;
                }

                _view.Render(snapshot, localPlayerId);
            }

            if (message.Type == NetworkProtocol.Error)
            {
                Console.Clear();
                Console.WriteLine("Server error:");
                Console.WriteLine(message.ErrorMessage);
                _running = false;
                return;
            }
        }
    }

    private GameSnapshotDto? GetSnapshot()
    {
        lock (_snapshotLock)
        {
            return _lastSnapshot;
        }
    }

    private async Task SendMessageAsync(NetworkMessageDto message)
    {
        if (_writer == null)
            return;

        string json = _serializer.Serialize(message);
        await _writer.WriteLineAsync(json);
    }
}