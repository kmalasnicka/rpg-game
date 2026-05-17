using System.Linq;
namespace Rpg;

public sealed class Game{
    private readonly Room _room;
    private readonly Player _player;
    private readonly Renderer _renderer;
    private readonly InstructionBuilder _instructionBuilder; //obiekt ktory buduje liste instrukcji do wyswietlenia
    private readonly CombatService _combatService;
    private readonly DungeonFeatures _features;

    private bool _running = true;
    private IGameMode _mode; 
    private int _selectedIndex;
    private string _message = "";
    private bool _gameOver;

    private readonly string _introMessage;
    private readonly string _logFilePath;
    private readonly string _themeName;
    private string _lastLogMessage = "";

    private readonly Subject<NoiseEvent> _noiseSubject;
    private readonly Random _random = new();

    public Game(Room room, Player player, Renderer renderer, DungeonFeatures features, string introMessage, string logFilePath, string themeName, Subject<NoiseEvent> noiseSubject){
        _room = room;
        _player = player;
        _renderer = renderer;
        _features = features;
        _introMessage = introMessage;
        _logFilePath = logFilePath;
        _themeName = themeName;
        _noiseSubject = noiseSubject;

        _mode = new WorldMode();
        _instructionBuilder = new InstructionBuilder();
        _combatService = new CombatService();
        IsInventoryOpen = false;
        _message = _introMessage;
    }

    public Room Room => _room;
    public Player Player => _player;
    public bool IsInventoryOpen { get; private set; }
    public DungeonFeatures Features => _features;
    public string ThemeName => _themeName;

    public void Run(){
        _renderer.Initialize(); //przygotowujemy konsole
        while (_running){
            if(_gameOver){
                _renderer.RenderGameOver(_message);
                Console.ReadKey(intercept: true);
                _running = false;
                break;
            }
            var actions = _mode.GetActions(this); //lista dostepnych akcji z aktualnego trybu
            var instructions = _instructionBuilder.Build(this, actions); //tworzymy instrukcje na podstawie trybu i stanu gry
            var recentLogEntries = EventLog.Current.GetRecentEntries(); //bierzemy ostatnie wpisy
            _renderer.Render(_room, _player, actions, _selectedIndex, _message, instructions, IsInventoryOpen, recentLogEntries, ThemeName);
            
            var key = Console.ReadKey(intercept: true).Key;
            HandleKey(key, actions);
            }
        _renderer.Shutdown();
    }

    private void HandleKey(ConsoleKey key, List<GameAction> actions){
        var action = actions.FirstOrDefault(a => a.Key == key); //szukamy akcji przypisanej do klawisza
        if (action == null){
            Log($"Unknown key pressed: {key}.");
            return;
        }

        if (!action.CanExecute(this)){
            _message = action.CannotExecuteMessage;
            return;
        }
        action.Execute(this);
    }

    public bool SupportsItems() => _features.HasItems;
    public bool SupportsWeapons() => _features.HasWeapons;
    public bool SupportsCurrency() => _features.HasCurrency;
    public bool SupportsEnemies() => _features.HasEnemies;
    public bool SupportsLoot() => _features.HasItems || _features.HasWeapons || _features.HasCurrency;

    public bool HasItemOnGround() => _room.GetCell(_player.Position).ItemsOnCell.Count > 0;
    public bool HasItemsAnywhere() => _room.HasAnyGroundItems();
    public bool HasInventory() => _player.Inventory.Count > 0;
    public bool HasLeftEquipped() => _player.Equipment.Left != null;
    public bool HasRightEquipped() => _player.Equipment.Right != null;
    public bool HasAnyEquipped() => _player.Equipment.Left != null || _player.Equipment.Right != null;
    public bool HasAnythingToUnequip() => HasAnyEquipped();
    public bool HasEnemyOnCurrentCell() => _room.GetCell(_player.Position).Enemy != null;
    public bool HasEnemiesAnywhere() => _room.HasAnyEnemies();

    public void OpenInventory(){
        if (!SupportsLoot()){
            _message = "Inventory is not available in this dungeon";
            return;
        }
        _mode = new InventoryMode();
        IsInventoryOpen = true;
        _selectedIndex = 0;
        _message = "Inventory opened";
    }

    public void CloseInventory(){
        _mode = new WorldMode();
        IsInventoryOpen = false;
        _message = "Inventory closed";
    }

    public void Quit(){
        _running = false;
    }

    public void SelectUp(){
        if (_selectedIndex > 0)
            _selectedIndex--;
    }

    public void SelectDown(){
        if (_selectedIndex < _player.Inventory.Count - 1)
            _selectedIndex++;
    }

    public void EquipSelected(bool left){
        if (_player.Inventory.Count == 0){
            _message = "Inventory is empty";
            return;
        }

        if (_selectedIndex < 0 || _selectedIndex >= _player.Inventory.Count){
            _message = "Invalid selection";
            return;
        }

        var item = _player.Inventory.Items[_selectedIndex];

        if (item.TryEquip(_player, left)){
            _player.Inventory.RemoveAt(_selectedIndex);
            if (_selectedIndex >= _player.Inventory.Count) _selectedIndex = Math.Max(0, _player.Inventory.Count - 1);
            _message = $"Equipped: {item.Name}";
            Log($"{_player.Name} equipped: {item.Name}.");
        } else {
            _message = $"Cannot equip: {item.Name}";
        }
    }

    public void UnequipLeft(){
        var item = _player.Equipment.RemoveLeft();
        if (item == null){
            _message = "Left hand is empty";
            return;
        }
        _player.Inventory.Add(item);
        _message = $"Unequipped: {item.Name}";
    }

    public void UnequipRight(){
        var item = _player.Equipment.RemoveRight();
        if (item == null){
            _message = "Right hand is empty";
            return;
        }
        _player.Inventory.Add(item);
        _message = $"Unequipped: {item.Name}";
    }

    public void UnequipBoth(){
        var left = _player.Equipment.Left;
        var right = _player.Equipment.Right;
        if (left == null && right == null){
            _message = "Both hands are empty";
            return;
        }
        _player.Equipment.Clear();
        if (left != null) _player.Inventory.Add(left);
        if (right != null && !ReferenceEquals(left, right)) _player.Inventory.Add(right);
        _message = "Unequipped both hands";
    }

    public void TryMove(int dx, int dy){
        var next = new Position(_player.Position.X + dx, _player.Position.Y + dy);
        if (_room.CanEnter(next)){
            _player.SetPosition(next);
            if (HasEnemyOnCurrentCell()) _message = "Enemy encountered. Press 1, 2 or 3 to attack.";
            else{
                _room.MoveEnemiesRandomly(_random, _player.Position);
                _message = "";
            }
        } else {
            _message = "You cannot move there";
            Log($"Player attempted to walk into a wall at {next}.");
        }
    }

    public void TryPickup(){
        var cell = _room.GetCell(_player.Position);
        if (cell.ItemsOnCell.Count == 0) {
            _message = "There is nothing here";
            return;
        }
        var item = cell.ItemsOnCell[0];
        cell.ItemsOnCell.RemoveAt(0);
        item.Interact(_player);
        _message = $"Picked up: {item.Name}";
        Log($"{_player.Name} picked up: {item.Name}.");

         if (item.NoiseRange > 0){
            _noiseSubject.NotifyAll(new NoiseEvent(_player.Position, item.NoiseRange, _room));
        }

        _room.MoveEnemiesRandomly(_random, _player.Position);
    }

    public void DropSelected(){
        if (_player.Inventory.Count == 0){
            _message = "Inventory is empty";
            return;
        }
        if (_selectedIndex < 0 || _selectedIndex >= _player.Inventory.Count){
            _message = "Invalid selection";
            return;
        }
        var item = _player.Inventory.RemoveAt(_selectedIndex);
        _room.GetCell(_player.Position).ItemsOnCell.Add(item);
        if (_selectedIndex >= _player.Inventory.Count) _selectedIndex = Math.Max(0, _player.Inventory.Count - 1);
        _message = $"Dropped: {item.Name}";
    }

    public void AttackEnemy(IAttackStyle style)
    {
        Cell cell = Room.GetCell(Player.Position);

        if (cell.Enemy == null)
        {
            _message = "There is no enemy here.";
            return;
        }

        Enemy enemy = cell.Enemy;
        CombatResult result = _combatService.PerformAttack(Player, enemy, style);
        _message = result.Message;

        if (result.EnemyDefeated)
        {
            enemy.BroadcastDeath();
            _room.UnregisterEnemy(enemy);
            cell.RemoveEnemy();

            _message = $"You defeated {enemy.Name}.";
            return;
        }

        if (result.PlayerDefeated || Player.IsDead)
        {
            _message = $"GAME OVER: You were defeated by {enemy.Name}. Log file: {_logFilePath}";
            Log($"{Player.Name} was defeated by {enemy.Name}.");
            Log($"Log file saved at: {_logFilePath}");
            _gameOver = true;
            return;
        }
    }

    public void OpenJournal()
    {
        var entries = EventLog.Current.GetAllEntries();
        _renderer.RenderJournal(entries);
    }

    private void Log(string message)
    {
        if (_lastLogMessage == message)
            return;

        EventLog.Current.Add(message);
        _lastLogMessage = message;
    }
}
