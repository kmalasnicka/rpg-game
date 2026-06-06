namespace Rpg;

public sealed class StartupOptions
{
    public string Mode { get; }
    public string Host { get; }
    public int Port { get; }

    private StartupOptions(string mode, string host, int port)
    {
        Mode = mode;
        Host = host;
        Port = port;
    }

    public static StartupOptions Parse(string[] args)
    {
        if (args.Length > 0)
            return ParseArguments(args);

        return AskUser();
    }

    private static StartupOptions ParseArguments(string[] args)
    {
        if (args[0] == "--server")
        {
            int port = 5555;

            if (args.Length >= 2 && !int.TryParse(args[1], out port))
                throw new ArgumentException("Invalid server port. Example: dotnet run -- --server 5555");

            ValidatePort(port);

            return new StartupOptions("server", "127.0.0.1", port);
        }

        if (args[0] == "--client")
        {
            string host = "127.0.0.1";
            int port = 5555;

            if (args.Length >= 2)
                ParseAddress(args[1], out host, out port);

            ValidatePort(port);

            return new StartupOptions("client", host, port);
        }

        throw new ArgumentException(
            "Unknown startup option. Use:\n" +
            "dotnet run -- --server 5555\n" +
            "or\n" +
            "dotnet run -- --client 127.0.0.1:5555"
        );
    }

    private static StartupOptions AskUser()
    {
        while (true)
        {
            Console.Write("Start as (S)erver or (C)lient? ");
            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Please enter S for server or C for client.");
                continue;
            }

            input = input.Trim();

            if (string.Equals(input, "S", StringComparison.OrdinalIgnoreCase))
                return new StartupOptions("server", "127.0.0.1", 5555);

            if (string.Equals(input, "C", StringComparison.OrdinalIgnoreCase))
                return new StartupOptions("client", "127.0.0.1", 5555);

            Console.WriteLine($"Invalid option: {input}");
            Console.WriteLine("Please enter only S or C.");
            Console.WriteLine();
        }
    }

    private static void ParseAddress(string text, out string host, out int port)
    {
        host = "127.0.0.1";
        port = 5555;

        if (string.IsNullOrWhiteSpace(text))
            return;

        string[] parts = text.Split(':');

        if (parts.Length > 2)
            throw new ArgumentException("Invalid client address. Example: 127.0.0.1:5555");

        if (parts.Length >= 1 && !string.IsNullOrWhiteSpace(parts[0]))
            host = parts[0].Trim();

        if (parts.Length == 2)
        {
            if (!int.TryParse(parts[1], out port))
                throw new ArgumentException("Invalid client port. Example: 127.0.0.1:5555");
        }
    }

    private static void ValidatePort(int port)
    {
        if (port < 1 || port > 65535)
            throw new ArgumentException("Port must be between 1 and 65535.");
    }
}