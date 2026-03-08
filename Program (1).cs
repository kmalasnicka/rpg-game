namespace Rpg;

public static class Program
{
    public static void Main()
    {
        var config = GameConfig.Default; 
        var room = Room.CreateDefault(config);
        var attributes = new Attributes(
            strength: 5,
            dexterity: 5,
            health: 10,
            luck: 3,
            aggression: 4,
            wisdom: 2
        );
        var player = new Player(config.PlayerStart, attributes);
        var renderer = new Renderer(); 
        var game = new Game(room, player, renderer);
        game.Run();
    }
}
