using System.Net.Sockets;

namespace Rpg;

public static class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            StartupOptions startupOptions = StartupOptions.Parse(args);

            var settings = GameSettings.Load("gameconfig.json");

            if (startupOptions.Mode == "client")
            {
                var client = new ClientApp(
                    startupOptions.Host,
                    startupOptions.Port,
                    settings.PlayerName
                );

                await client.StartAsync();
                return;
            }

            var log = new FileEventLog(settings.LogDirectory, settings.PlayerName, DateTime.Now);
            EventLog.Instance.Configure(log);

            GameModel model = CreateModel(settings, log.FilePath);
            var controller = new GameController(model);

            var server = new ServerApp(
                startupOptions.Port,
                model,
                controller
            );

            await server.StartAsync(settings.PlayerName);
        }
        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.ConnectionRefused)
        {
            PrintError(
                "Cannot connect to the server.",
                "Make sure the server is running first.",
                "Start server in another terminal:",
                "dotnet run -- --server 5555",
                "",
                "Then start client:",
                "dotnet run -- --client 127.0.0.1:5555"
            );
        }
        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.AddressAlreadyInUse)
        {
            PrintError(
                "Cannot start server.",
                "Port is already in use. Probably another server is already running.",
                "Close the previous server or use another port:",
                "dotnet run -- --server 5556",
                "",
                "Then connect client to the same port:",
                "dotnet run -- --client 127.0.0.1:5556"
            );
        }
        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.HostNotFound)
        {
            PrintError(
                "Cannot find the server host.",
                "Check the IP address.",
                "Example:",
                "dotnet run -- --client 127.0.0.1:5555"
            );
        }
        catch (SocketException ex) when (ex.SocketErrorCode == SocketError.TimedOut)
        {
            PrintError(
                "Connection timed out.",
                "The server did not respond.",
                "Check if the server is running and if the address is correct."
            );
        }
        catch (ArgumentException ex)
        {
            PrintError(
                "Invalid startup option.",
                ex.Message
            );
        }
        catch (FileNotFoundException ex)
        {
            PrintError(
                "Configuration file was not found.",
                ex.Message,
                "Make sure gameconfig.json exists in the project directory."
            );
        }
        catch (IOException)
        {
            PrintError(
                "Connection was lost.",
                "The server or client probably closed the connection."
            );
        }
        catch (Exception ex)
        {
            PrintError(
                "Unexpected error.",
                ex.Message
            );
        }
    }

    private static GameModel CreateModel(GameSettings settings, string logFilePath)
    {
        var config = GameConfig.Default;

        var themeSelector = new DungeonThemeSelector();
        IDungeonTheme theme = themeSelector.Select(settings.Theme);

        EventLog.Current.Add($"Game started for player: {settings.PlayerName}.");
        EventLog.Current.Add($"Selected dungeon theme: {theme.Name}.");
        EventLog.Current.Add(theme.IntroMessage);

        var builder = new DungeonBuilder(config.GridWidth, config.GridHeight);

        IDungeonStrategy strategy = theme.CreateStrategy();
        strategy.Build(builder);

        Room room = builder.Build();

        return new GameModel(
            room,
            builder.Features,
            theme.Name,
            logFilePath,
            builder.NoiseSubject
        );
    }

    private static void PrintError(params string[] lines)
    {
        Console.WriteLine();
        Console.WriteLine("ERROR");
        foreach (string line in lines)
            Console.WriteLine(line);

    }
}