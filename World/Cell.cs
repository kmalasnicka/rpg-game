namespace Rpg;

public class Cell{
    public Tile Tile { get; private set; }
    public List<Item> ItemsOnCell { get; } = new();
    public Enemy? Enemy { get; private set; }

    public Cell(Tile tile){
        Tile = tile;
    }

    public void SetTile(Tile tile){
        Tile = tile;
        if (!tile.CanEnter){
            ItemsOnCell.Clear();
            Enemy = null;
        }
    }

    public bool HasEnemy() => Enemy != null;
    
    public void SetEnemy(Enemy enemy){
        Enemy = enemy;
    }

    public void RemoveEnemy(){
        Enemy = null;
    }
}