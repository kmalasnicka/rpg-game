namespace Rpg;

public abstract class Tile : IDrawable{
    public abstract char Symbol { get; } 
    public virtual bool CanEnter => true; //domyslnie mozna wejsc na pole 
}

public class EmptyTile : Tile{
    public override char Symbol => ' ';
}

public class WallTile : Tile{
    public override char Symbol => '█';
    public override bool CanEnter => false;
}