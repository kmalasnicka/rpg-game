namespace Rpg;

public sealed class Game
{
    private readonly Room _room;
    private readonly Player _player;
    private readonly Renderer _renderer;
    private bool _running = true; 
    private bool _inventoryOpen = false;
    private int _selectedIndex = 0;
    private string _message = "";

    public Game(Room room, Player player, Renderer renderer)
    {
        _room = room;
        _player = player;
        _renderer = renderer;
    }

    public void Run()
    {
        _renderer.Initialize();
        while (_running){
            _renderer.Render(_room, _player, _inventoryOpen, _selectedIndex, _message);
            var key = Console.ReadKey(intercept: true).Key; //intercept: true - nie beda sie wypisywac litery po nacisnieciu
            if(_inventoryOpen) HandleInventoryKey(key);
            else HandleWorldKey(key);
        }
        _renderer.Shutdown();
    }

    private void HandleWorldKey(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.Q: //konczy gre
                _running = false;
                return;
            case ConsoleKey.I: 
                _inventoryOpen = true;
                _selectedIndex = 0;
                return;
            case ConsoleKey.W:
                TryMove(0, -1);
                return;
            case ConsoleKey.S:
                TryMove(0, 1);
                return;
            case ConsoleKey.A:
                TryMove(-1, 0); 
                return;
            case ConsoleKey.D:
                TryMove(1, 0);
                return;
            case ConsoleKey.E:
                TryPickup();
                return;
        }
    }

    private void HandleInventoryKey(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.Escape:
                _inventoryOpen = false;
                return;
            case ConsoleKey.UpArrow:
                if (_player.Inventory.Count > 0)
                    _selectedIndex = Math.Max(0, _selectedIndex - 1); //zabezpieczamy zejscie ponizej 0
                return;
            case ConsoleKey.DownArrow:
                if (_player.Inventory.Count > 0)
                    _selectedIndex = Math.Min(_player.Inventory.Count - 1, _selectedIndex + 1); //zabezpieczamy wyjscie za ostatni element inventory
                return;
            case ConsoleKey.L:
                TryEquip(targetLeftHand: true);
                return;
            case ConsoleKey.R:
                TryEquip(targetLeftHand: false);
                return;
            case ConsoleKey.Z: //zdejmujemy item z lewej reki do inventory
                UnequipLeftToInventory();
                return;
            case ConsoleKey.C: //zdejmujemy item z prawej reki do inventory
                UnequipRightToInventory();
                return;
            case ConsoleKey.U: //zdejmujemy oba do inventory
                UnequipBothToInventory();
                return;
            case ConsoleKey.X:
                DropSelected();
                return;
        }
    }

    private void TryMove(int dx, int dy){
        var target = _player.Position.Move(dx, dy);
        if (!_room.IsInside(target)){
            _message = "Out of bounds";
            return;
        }
        if (!_room.CanEnter(target)){
            _message = "A wall blocks the way";
            return;
        }
        _player.SetPosition(target);
    }

    private void TryPickup(){
        var cell = _room.GetCell(_player.Position);
        if (cell.ItemsOnCell.Count == 0){
            _message = "There is nothing to pick up";
            return;
        }
        var item = cell.ItemsOnCell[0];
        cell.ItemsOnCell.RemoveAt(0);
        item.Interact(_player);
        _message = $"Picked up: {item.Name}";
    }

    private void TryEquip(bool targetLeftHand){
        if (_player.Inventory.Count == 0){
            _message = "Inventory is empty";
            return;
        }
        if (_selectedIndex < 0 || _selectedIndex >= _player.Inventory.Count){
            _message = "Invalid selection";
            return;
        }
        var item = _player.Inventory.Items[_selectedIndex];
        if (!item.TryEquip(_player, targetLeftHand)){
            _message = "This item cannot be equipped";
            return;
        }
        _player.Inventory.RemoveAt(_selectedIndex);
        if (_selectedIndex >= _player.Inventory.Count)
            _selectedIndex = Math.Max(0, _player.Inventory.Count - 1);
        _message = $"Equipped: {item.Name}";
    }

    private void DropSelected(){
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
        if (_selectedIndex >= _player.Inventory.Count)
            _selectedIndex = Math.Max(0, _player.Inventory.Count - 1);
        _message = $"Dropped: {item.Name}";
    }

    private void UnequipLeftToInventory()
    {
        var item = _player.Equipment.RemoveLeft();
        if (item == null){
            _message = "Left hand is empty";
            return;
        }
        _player.Inventory.Add(item);
        _message = $"Unequipped from left hand: {item.Name}";
    }

    private void UnequipRightToInventory()
    {
        var item = _player.Equipment.RemoveRight();
        if (item == null){
            _message = "Right hand is empty";
            return;
        }
        _player.Inventory.Add(item);
        _message = $"Unequipped from right hand: {item.Name}";
    }

    private void UnequipBothToInventory(){
        var left = _player.Equipment.Left;
        var right = _player.Equipment.Right;
        if (left == null && right == null){
            _message = "Both hands are empty";
            return;
        }
        _player.Equipment.Clear();
        if (left != null) _player.Inventory.Add(left);
        if (right != null && !ReferenceEquals(right, left)) _player.Inventory.Add(right);
        _message = "Unequipped to inventory";
    }
}