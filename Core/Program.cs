namespace Rpg;

public static class Program{
    public static void Main(){
        var config = GameConfig.Default;
        var builder = new DungeonBuilder(config.GridWidth, config.GridHeight); //builder do skladania dungeonu

        var strategy = new DungeonGroundsStrategy( 
            itemsCount: 10,
            weaponsCount: 10,
            coinsCount: 5,
            goldCount: 5,
            enemiesCount: 5);
        //builder tworzy nowy dungeon
        strategy.Build(builder);
        var room = builder.Build();

        var attributes = new Attributes(
            strength: 5,
            dexterity: 5,
            luck: 15,
            aggression: 4,
            wisdom: 2);
//jesli (0,0) jest przechodnie to tam startujemy jak nie to z pierwszego przechoniego pola z mapy
        var start = room.CanEnter(config.PlayerStart) ? config.PlayerStart : room.GetWalkablePositions().FirstOrDefault();
        if (!room.CanEnter(start)) throw new InvalidOperationException("Dungeon does not contain any walkable starting position.");

        var player = new Player(start, attributes, 30);
        var renderer = new Renderer();
        var game = new Game(room, player, renderer, builder.Features);

        game.Run();
    }
}