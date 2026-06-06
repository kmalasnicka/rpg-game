using System.Net;
using System.Net.Sockets;

namespace Rpg;

public sealed class ServerApp
{
    private const int MaxPlayers = 9;

    private readonly int _port;
    private readonly GameModel _model;
    private readonly GameController _controller;
    private readonly ConsoleGameView _view = new();
    private readonly ConsoleInputController _inputController = new();

    private readonly List<ClientConnection> _clients = new();
    private readonly object _clientsLock = new();

    private TcpListener? _listener;
    private bool _running = true;

    public ServerApp(int port, GameModel model, GameController controller)
    {
        _port = port;
        _model = model;
        _controller = controller;
    }

    public async Task StartAsync(string hostPlayerName)
    {
        int hostId = _model.AddPlayer(hostPlayerName);

        _listener = new TcpListener(IPAddress.Any, _port);
        _listener.Start();

        _ = Task.Run(AcceptClientsLoopAsync);

        _view.Initialize();

        while (_running)
        {
            GameSnapshotDto snapshot = _model.CreateSnapshot();
            _view.Render(snapshot, hostId);

            ConsoleKey key = Console.ReadKey(intercept: true).Key;
            PlayerActionDto? action = _inputController.ReadAction(key, snapshot, hostId);

            if (action == null)
                continue;

            if (action.ActionName == "Quit")
            {
                _running = false;
                break;
            }

            _controller.HandleAction(action);
            await BroadcastAsync();
        }

        _listener.Stop();
        _view.Shutdown();
    }

    private async Task AcceptClientsLoopAsync()
    {
        if (_listener == null)
            return;

        while (_running)
        {
            TcpClient tcpClient;

            try
            {
                tcpClient = await _listener.AcceptTcpClientAsync();
            }
            catch
            {
                return;
            }

            if (_model.PlayerCount >= MaxPlayers)
            {
                await RejectClientAsync(tcpClient, "Server is full. Maximum number of players is 9.");
                continue;
            }

            _ = Task.Run(() => HandleNewClientAsync(tcpClient));
        }
    }

    private async Task HandleNewClientAsync(TcpClient tcpClient)
    {
        var serializer = new JsonMessageSerializer();
        using NetworkStream stream = tcpClient.GetStream();
        using var reader = new StreamReader(stream);
        using var writer = new StreamWriter(stream)
        {
            AutoFlush = true
        };

        string? loginJson = await reader.ReadLineAsync();

        if (loginJson == null)
        {
            tcpClient.Close();
            return;
        }

        NetworkMessageDto? loginMessage = serializer.Deserialize(loginJson);

        if (loginMessage == null || loginMessage.Type != NetworkProtocol.Login || loginMessage.Login == null)
        {
            tcpClient.Close();
            return;
        }

        int playerId = _model.AddPlayer(loginMessage.Login.PlayerName);
        var connection = new ClientConnection(tcpClient, playerId, loginMessage.Login.PlayerName);

        lock (_clientsLock)
        {
            _clients.Add(connection);
        }

        await connection.SendUpdateAsync(_model.CreateSnapshot());
        await BroadcastAsync();

        await ClientMessagesLoopAsync(connection);
    }

    private async Task ClientMessagesLoopAsync(ClientConnection connection)
    {
        try
        {
            while (_running)
            {
                NetworkMessageDto? message = await connection.ReadMessageAsync();

                if (message == null)
                    break;

                if (message.Type != NetworkProtocol.Action || message.Action == null)
                    continue;

                message.Action.PlayerId = connection.PlayerId;

                if (message.Action.ActionName == "Disconnect")
                    break;

                _controller.HandleAction(message.Action);
                await BroadcastAsync();
            }
        }
        finally
        {
            _model.RemovePlayer(connection.PlayerId);

            lock (_clientsLock)
            {
                _clients.Remove(connection);
            }

            connection.Close();
            await BroadcastAsync();
        }
    }

    private async Task BroadcastAsync()
    {
        GameSnapshotDto snapshot = _model.CreateSnapshot();

        _view.Render(snapshot, 1);

        List<ClientConnection> clientsCopy;

        lock (_clientsLock)
        {
            clientsCopy = _clients.ToList();
        }

        foreach (ClientConnection client in clientsCopy)
        {
            try
            {
                await client.SendUpdateAsync(snapshot);
            }
            catch
            {
                client.Close();

                lock (_clientsLock)
                {
                    _clients.Remove(client);
                }

                _model.RemovePlayer(client.PlayerId);
            }
        }
    }

    private async Task RejectClientAsync(TcpClient tcpClient, string reason)
    {
        try
        {
            var serializer = new JsonMessageSerializer();

            using NetworkStream stream = tcpClient.GetStream();
            using var writer = new StreamWriter(stream)
            {
                AutoFlush = true
            };

            var message = new NetworkMessageDto
            {
                Type = NetworkProtocol.Error,
                ErrorMessage = reason
            };

            string json = serializer.Serialize(message);
            await writer.WriteLineAsync(json);
        }
        finally
        {
            tcpClient.Close();
        }
    }
}