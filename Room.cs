namespace Rpg;

public class Room{
    private readonly Cell[,] _cells; 
    public int Height {get;}
    public int Width {get;}

    public Room(int height, int width){
        Height = height;
        Width = width;
        _cells = new Cell[height, width];

        for(int y = 0; y < height; y++){
            for(int x = 0; x < width; x++){
                _cells[y, x] = new Cell(new EmptyTile()); //cala plansza ma puste kafelki 
            }
        }
    }
    public static Room CreateDefault(GameConfig config) {
    var room = new Room(config.GridHeight, config.GridWidth);
    //sciany
    room.SetTile(new Position(6, 0), new WallTile());
    room.SetTile(new Position(6, 1), new WallTile());
    room.SetTile(new Position(6, 2), new WallTile());
    room.SetTile(new Position(6, 3), new WallTile());
    room.SetTile(new Position(6, 4), new WallTile());
    room.SetTile(new Position(6, 5), new WallTile());

    room.SetTile(new Position(12, 6), new WallTile());
    room.SetTile(new Position(13, 6), new WallTile());
    room.SetTile(new Position(14, 6), new WallTile());
    room.SetTile(new Position(15, 6), new WallTile());
    room.SetTile(new Position(16, 6), new WallTile());
    room.SetTile(new Position(17, 6), new WallTile());
    room.SetTile(new Position(18, 6), new WallTile());
    room.SetTile(new Position(19, 6), new WallTile());

    room.SetTile(new Position(25, 10), new WallTile());
    room.SetTile(new Position(25, 11), new WallTile());
    room.SetTile(new Position(25, 12), new WallTile());
    room.SetTile(new Position(25, 13), new WallTile());


    room.SetTile(new Position(15, 19), new WallTile());
    room.SetTile(new Position(16, 19), new WallTile());
    room.SetTile(new Position(17, 19), new WallTile());
    room.SetTile(new Position(18, 19), new WallTile());
    room.SetTile(new Position(19, 19), new WallTile());
    room.SetTile(new Position(20, 19), new WallTile());
    room.SetTile(new Position(21, 19), new WallTile());

    // pojedyncze itemy
    room.PlaceItem(new Position(1, 0), new Coin());
    room.PlaceItem(new Position(2, 0), new Gold());
    room.PlaceItem(new Position(3, 1), new Dagger());
    room.PlaceItem(new Position(4, 2), new Sword());
    room.PlaceItem(new Position(8, 3), new Axe());
    room.PlaceItem(new Position(10, 1), new Useless1());
    room.PlaceItem(new Position(14, 4), new Useless2());

    // pola z wieloma itemami
    room.PlaceItem(new Position(20, 8), new Useless3());
    room.PlaceItem(new Position(20, 8), new Coin());
    room.PlaceItem(new Position(20, 8), new Gold());

    room.PlaceItem(new Position(5, 8), new Coin());
    room.PlaceItem(new Position(5, 8), new Coin());
    room.PlaceItem(new Position(5, 8), new Gold());

    room.PlaceItem(new Position(9, 10), new Dagger());
    room.PlaceItem(new Position(9, 10), new Useless1());
    room.PlaceItem(new Position(9, 10), new Coin());

    room.PlaceItem(new Position(15, 2), new Sword());
    room.PlaceItem(new Position(15, 2), new Gold());

    room.PlaceItem(new Position(22, 5), new Axe());
    room.PlaceItem(new Position(22, 5), new Useless2());

    room.PlaceItem(new Position(30, 15), new Useless1());
    room.PlaceItem(new Position(30, 15), new Useless2());
    room.PlaceItem(new Position(30, 15), new Useless3());

    room.PlaceItem(new Position(35, 18), new Coin());
    room.PlaceItem(new Position(35, 18), new Gold());
    room.PlaceItem(new Position(35, 18), new Dagger());

    return room;
    }
    public Cell GetCell(Position position) => _cells[position.Y, position.X];
    public bool IsInside(Position position) => position.X >= 0 && position.X < Width && position.Y >= 0 && position.Y < Height;
    public bool CanEnter(Position position) => IsInside(position) && GetCell(position).Tile.CanEnter;  
    public void SetTile(Position position, Tile tile){ //podnieniamy kafelek w komorce 
        _cells[position.Y, position.X] = new Cell(tile);
    }
    public void PlaceItem(Position position, Item item){ //dodajemy item do listy itemow w komorce 
        GetCell(position).ItemsOnCell.Add(item);
    }
}

public class Cell{
    public Tile Tile { get; } 
    public List<Item> ItemsOnCell { get; } = new(); 
    public Cell(Tile tile){
        Tile = tile;
    }
}