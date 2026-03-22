namespace Rpg;

public class Room
{
    private readonly Cell[,] _cells;
    private readonly List<Item> _pendingItems = new();
    public int Height { get; }
    public int Width { get; }

    public Room(int width, int height){
        Width = width;
        Height = height;
        _cells = new Cell[height, width];
        for (int y = 0; y < height; y++){
            for (int x = 0; x < width; x++){
                _cells[y, x] = new Cell(new EmptyTile());
            }
        }
    }

    public Cell GetCell(Position position) => _cells[position.Y, position.X];
    public bool IsInside(Position position) => position.X >= 0 && position.X < Width && position.Y >= 0 && position.Y < Height;
    public bool CanEnter(Position position) => IsInside(position) && GetCell(position).Tile.CanEnter;

    public void SetTile(Position position, Tile tile){
        _cells[position.Y, position.X].SetTile(tile);
    }

    public void PlaceItem(Position position, Item item){ //dodaje item na pozycje
        if (!CanEnter(position)){
            _pendingItems.Add(item);
            return;
        }
        GetCell(position).ItemsOnCell.Add(item);
    }

    public void QueueItem(Item item){ 
        _pendingItems.Add(item);
    }
//umieszczanie czekajacych przedmiotow na losowych dosteonych polach
    public void TryPlacePendingItems(Random random){
        if (_pendingItems.Count == 0) return;
        var walkablePositions = GetWalkablePositions().ToList();
        if (walkablePositions.Count == 0) return;
        foreach (var item in _pendingItems){
            var position = walkablePositions[random.Next(walkablePositions.Count)]; //losuje pole
            GetCell(position).ItemsOnCell.Add(item);
        }
        _pendingItems.Clear();
    }

    public IEnumerable<Position> GetAllPositions(){ //zwraca wszystkie pozycje w pokoju
        for (int y = 0; y < Height; y++) {
            for (int x = 0; x < Width; x++){
                yield return new Position(x, y);
            }
        }
    }

    public IEnumerable<Position> GetWalkablePositions(){ //pozycje ktore nie sa sciana 
        foreach (var position in GetAllPositions()){
            if (CanEnter(position))
                yield return position;
        }
    }

    public bool HasAnyGroundItems(){ //czy jest jakikolwiek przedmiot w pokoju
        foreach (var position in GetAllPositions()){
            if (GetCell(position).ItemsOnCell.Count > 0) return true;
        }
        return false;
    }

    public int CountGroundItems(){ //zlicza wszystkie przedmioty w pokoju
        int count = 0;
        foreach (var position in GetAllPositions()) count += GetCell(position).ItemsOnCell.Count;
        return count;
    }
    public int CountPendingItems() => _pendingItems.Count; //liczba przedmiotow czekajacyhc na umieszczenie
}

public class Cell{
    public Tile Tile { get; private set; }
    public List<Item> ItemsOnCell { get; } = new();
    public Cell(Tile tile){
        Tile = tile;
    }
    public void SetTile(Tile tile){
        Tile = tile;
        if (!tile.CanEnter) ItemsOnCell.Clear();
    }
}