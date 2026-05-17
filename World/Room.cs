namespace Rpg;

public class Room
{
    private readonly Cell[,] _cells;
    private readonly List<Item> _pendingItems = new();
    public int Height { get; }
    public int Width { get; }
    private readonly List<Enemy> _enemies = new();

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

    public Position? FindRandomEmptyWalkablePositionWithoutEnemy(Random random){
        var positions = GetWalkablePositions().Where(position => !GetCell(position).HasEnemy()).ToList();
        if (positions.Count == 0) return null;
        return positions[random.Next(positions.Count)];
    }

    public bool HasAnyEnemies(){
        foreach (var position in GetAllPositions()){
            if (GetCell(position).HasEnemy()) return true;
        }
        return false;
    }

    public void RegisterEnemy(Enemy enemy){
        if (!_enemies.Contains(enemy)) _enemies.Add(enemy);
    }

    public void UnregisterEnemy(Enemy enemy){
        _enemies.Remove(enemy);
        enemy.RemoveSubscriptions();
    }

    public IReadOnlyList<Enemy> GetEnemies(){
        return _enemies;
    }

    public int? FindDistance(Position start, Position target, int maxDistance)
    {
        var visited = new HashSet<Position>(); //zbior odwiedzonych pol
        var queue = new Queue<(Position Position, int Distance)>();

        //start jest odwiedzony z dystansem 0
        visited.Add(start); 
        queue.Enqueue((start, 0));

        while (queue.Count > 0){ //dopoki sa pola do sprawdzenia 
            var current = queue.Dequeue(); //pierwsze pole z kolejki

            if (current.Position.X == target.X && current.Position.Y == target.Y) 
                return current.Distance; //znaleziono target w zasiegu maxDistance

            if (current.Distance >= maxDistance) continue; //jesli doszlismy do limitu to nie idziemy dalej

            foreach (var next in GetNeighbours(current.Position)){ //sasiednie pola
                if (visited.Contains(next)) continue; //juz bylismy na tym polu
                visited.Add(next);
                queue.Enqueue((next, current.Distance + 1));
            }
        }
        return null;
    }

    private IEnumerable<Position> GetNeighbours(Position position){ //zwraca dostepnych sasiadow danej pozycji
        var positions = new[]{
            position.Move(1, 0),
            position.Move(-1, 0),
            position.Move(0, 1),
            position.Move(0, -1)
        };

        foreach (var next in positions){
            if (CanEnter(next)) yield return next;  //jesli mozemy wejsc na te pole to je zwracamy
        }
    }

    public void MoveEnemiesRandomly(Random random, Position playerPosition) //losowe przesuwanie przeciwnikow po planszy
    {
        foreach (var enemy in _enemies.ToList())
        {
            if (enemy.Position.X == playerPosition.X && enemy.Position.Y == playerPosition.Y) continue; //jesli enemy stoi na tym samym polu co gracz to si enie ruszamy
            if (FindDistance(enemy.Position, playerPosition, 3) != null) continue; //jesli gracz w odleglosci 3 pol to sie nie ruszamy
            var possiblePositions = GetNeighbours(enemy.Position).Where(position => !GetCell(position).HasEnemy()).Where(position => position.X != playerPosition.X || position.Y != playerPosition.Y).ToList();
            if (possiblePositions.Count == 0) continue; 
            var newPosition = possiblePositions[random.Next(possiblePositions.Count)]; //losujemy jedna z mozliwych pozycji

            //usuwamy enemy ze starego pola i aktualizujemy nowa pozycje
            GetCell(enemy.Position).RemoveEnemy();
            GetCell(newPosition).SetEnemy(enemy);
            enemy.SetPosition(newPosition);
        }
    }

    public Position? FindStepTowards(Position start, Position target, int maxDistance) //szuka pierwszego kroku ktory trzeba wykonac zeby dosc do celu
    {
        var visited = new HashSet<Position>();
        var queue = new Queue<(Position Position, Position FirstStep, int Distance)>(); 
        var neighbours = GetNeighbours(start).ToList();
        
        for (int i = 0; i < neighbours.Count; i++)
        {
            var neighbour = neighbours[i];
            bool neighbourIsTarget = neighbour.X == target.X && neighbour.Y == target.Y; //sprawdzamy czy sasiad to cel
            if (GetCell(neighbour).HasEnemy() && !neighbourIsTarget) continue; //jesli na polu jest enemy i to nie jest target to skipujemy 

            visited.Add(neighbour);
            queue.Enqueue((neighbour, neighbour, 1));
        }

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current.Position.X == target.X && current.Position.Y == target.Y) 
                return current.FirstStep;

            if (current.Distance >= maxDistance) continue;
            var nextPositions = GetNeighbours(current.Position).ToList();

            for (int i = 0; i < nextPositions.Count; i++)
            {
                var next = nextPositions[i];
                bool nextIsTarget = next.X == target.X && next.Y == target.Y;
                if (visited.Contains(next)) continue;
                if (GetCell(next).HasEnemy() && !nextIsTarget) continue;
                visited.Add(next);
                queue.Enqueue((next, current.FirstStep, current.Distance + 1));
            }
        }

        return null;
    }
}