namespace Rpg;

public static class Program{
    public static void Main(){
        var settings = GameSettings.Load("gameconfig.json");
        var log = new FileEventLog(settings.LogDirectory, settings.PlayerName, DateTime.Now);
        EventLog.Instance.Configure(log);

        var config = GameConfig.Default;
        var themeSelector = new DungeonThemeSelector();
        var themes = new[] { "library", "forge", "treasury" };
        var randomTheme = themes[new Random().Next(themes.Length)];
        IDungeonTheme theme = themeSelector.Select(randomTheme);

        EventLog.Current.Add($"Game started for player: {settings.PlayerName}.");
        EventLog.Current.Add($"Selected dungeon theme: {theme.Name}.");
        EventLog.Current.Add(theme.IntroMessage);

        var builder = new DungeonBuilder(config.GridWidth, config.GridHeight); //builder do skladania dungeonu

        var strategy = theme.CreateStrategy();
        strategy.Build(builder);
        var room = builder.Build();

        var attributes = new Attributes(strength: 5, dexterity: 5, luck: 15, aggression: 4, wisdom: 2);

        var start = room.CanEnter(config.PlayerStart) ? config.PlayerStart : room.GetWalkablePositions().FirstOrDefault();
        if (!room.CanEnter(start)) throw new InvalidOperationException("Dungeon does not contain any walkable starting position.");

        var player = new Player(settings.PlayerName, start, attributes, 30);
        var renderer = new Renderer();
        var game = new Game(room, player, renderer, builder.Features, theme.IntroMessage, log.FilePath, theme.Name, builder.NoiseSubject);

        game.Run();
    }
}