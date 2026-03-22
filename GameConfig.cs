namespace Rpg;

public class GameConfig{
    public int GridWidth { get; }
    public int GridHeight { get; }
    public Position PlayerStart { get; }

    public GameConfig(int width, int height, Position position){
        GridWidth = width;
        GridHeight = height;
        PlayerStart = position;
    }
    public static GameConfig Default => new GameConfig(40, 20, new Position(0, 0));
}
