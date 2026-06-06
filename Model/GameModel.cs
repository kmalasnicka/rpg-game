namespace Rpg;

public sealed class GameModel
{
    private const int MaxPlayers = 9;

    private readonly Room _room;
    private readonly DungeonFeatures _features;
    private readonly string _themeName;
    private readonly string _logFilePath;
    private readonly Subject<NoiseEvent> _noiseSubject;
    private readonly CombatService _combatService = new();
    private readonly Random _random = new();
    private readonly object _lock = new();

    private readonly Dictionary<int, PlayerState> _players = new();

    public GameModel(
        Room room,
        DungeonFeatures features,
        string themeName,
        string logFilePath,
        Subject<NoiseEvent> noiseSubject)
    {
        _room = room;
        _features = features;
        _themeName = themeName;
        _logFilePath = logFilePath;
        _noiseSubject = noiseSubject;
    }

    public int PlayerCount
    {
        get
        {
            lock (_lock)
            {
                return _players.Count;
            }
        }
    }

    public int AddPlayer(string name)
    {
        lock (_lock)
        {
            if (_players.Count >= MaxPlayers)
                throw new InvalidOperationException("Server is full. Maximum number of players is 9.");

            int id = FindFreePlayerId();

            Position start = FindFreeStartPosition();

            var attributes = new Attributes(
                strength: 5,
                dexterity: 5,
                luck: 15,
                aggression: 4,
                wisdom: 2
            );

            var player = new Player(name, start, attributes, 30);

            _players.Add(id, new PlayerState(player)
            {
                Message = $"Welcome, {name}. You are player {id}."
            });

            EventLog.Current.Add($"{name} joined the game as player {id}.");

            return id;
        }
    }

    public void RemovePlayer(int playerId)
    {
        lock (_lock)
        {
            if (!_players.TryGetValue(playerId, out PlayerState? state))
                return;

            EventLog.Current.Add($"{state.Player.Name} left the game.");
            _players.Remove(playerId);
        }
    }

    public void SetMessage(int playerId, string message)
    {
        lock (_lock)
        {
            if (!_players.TryGetValue(playerId, out PlayerState? state))
                return;

            state.Message = message;
        }
    }

    public void MovePlayer(int playerId, int dx, int dy)
    {
        lock (_lock)
        {
            if (!_players.TryGetValue(playerId, out PlayerState? state))
                return;

            Player player = state.Player;

            if (player.IsDead)
            {
                state.Message = "You are dead.";
                return;
            }

            Position next = player.Position.Move(dx, dy);

            if (!_room.CanEnter(next))
            {
                state.Message = "You cannot move there.";
                EventLog.Current.Add($"{player.Name} tried to walk into a wall at {next}.");
                return;
            }

            if (IsOccupiedByOtherPlayer(playerId, next))
            {
                state.Message = "Another player is standing there.";
                return;
            }

            player.SetPosition(next);

            Cell cell = _room.GetCell(player.Position);

            if (cell.Enemy != null)
            {
                state.Message = "Enemy encountered. Press 1, 2 or 3 to attack.";
                return;
            }

            _room.MoveEnemiesRandomly(_random, player.Position);
            state.Message = "";
        }
    }

    public void PickUpItem(int playerId)
    {
        lock (_lock)
        {
            if (!_players.TryGetValue(playerId, out PlayerState? state))
                return;

            Player player = state.Player;
            Cell cell = _room.GetCell(player.Position);

            if (cell.ItemsOnCell.Count == 0)
            {
                state.Message = "There is nothing here.";
                return;
            }

            Item item = cell.ItemsOnCell[0];
            cell.ItemsOnCell.RemoveAt(0);

            item.Interact(player);

            state.Message = $"Picked up: {item.Name}";
            EventLog.Current.Add($"{player.Name} picked up: {item.Name}.");

            if (item.NoiseRange > 0)
                _noiseSubject.NotifyAll(new NoiseEvent(player.Position, item.NoiseRange, _room));

            _room.MoveEnemiesRandomly(_random, player.Position);
        }
    }

    public void OpenInventory(int playerId)
    {
        lock (_lock)
        {
            if (!_players.TryGetValue(playerId, out PlayerState? state))
                return;

            if (!SupportsLoot())
            {
                state.Message = "Inventory is not available in this dungeon.";
                return;
            }

            state.IsInventoryOpen = true;
            state.SelectedIndex = 0;
            state.Message = "Inventory opened.";
        }
    }

    public void CloseInventory(int playerId)
    {
        lock (_lock)
        {
            if (!_players.TryGetValue(playerId, out PlayerState? state))
                return;

            state.IsInventoryOpen = false;
            state.Message = "Inventory closed.";
        }
    }

    public void SelectUp(int playerId)
    {
        lock (_lock)
        {
            if (!_players.TryGetValue(playerId, out PlayerState? state))
                return;

            if (state.SelectedIndex > 0)
                state.SelectedIndex--;
        }
    }

    public void SelectDown(int playerId)
    {
        lock (_lock)
        {
            if (!_players.TryGetValue(playerId, out PlayerState? state))
                return;

            int count = state.Player.Inventory.Count;

            if (state.SelectedIndex < count - 1)
                state.SelectedIndex++;
        }
    }

    public void EquipSelected(int playerId, bool leftHand)
    {
        lock (_lock)
        {
            if (!_players.TryGetValue(playerId, out PlayerState? state))
                return;

            Player player = state.Player;

            if (player.Inventory.Count == 0)
            {
                state.Message = "Inventory is empty.";
                return;
            }

            if (state.SelectedIndex < 0 || state.SelectedIndex >= player.Inventory.Count)
            {
                state.Message = "Invalid selection.";
                return;
            }

            Item item = player.Inventory.Items[state.SelectedIndex];

            if (!item.TryEquip(player, leftHand))
            {
                state.Message = $"Cannot equip: {item.Name}";
                return;
            }

            player.Inventory.RemoveAt(state.SelectedIndex);

            if (state.SelectedIndex >= player.Inventory.Count)
                state.SelectedIndex = Math.Max(0, player.Inventory.Count - 1);

            state.Message = $"Equipped: {item.Name}";
            EventLog.Current.Add($"{player.Name} equipped {item.Name}.");
        }
    }

    public void UnequipLeft(int playerId)
    {
        lock (_lock)
        {
            if (!_players.TryGetValue(playerId, out PlayerState? state))
                return;

            Item? item = state.Player.Equipment.RemoveLeft();

            if (item == null)
            {
                state.Message = "Left hand is empty.";
                return;
            }

            state.Player.Inventory.Add(item);
            state.Message = $"Unequipped: {item.Name}";
        }
    }

    public void UnequipRight(int playerId)
    {
        lock (_lock)
        {
            if (!_players.TryGetValue(playerId, out PlayerState? state))
                return;

            Item? item = state.Player.Equipment.RemoveRight();

            if (item == null)
            {
                state.Message = "Right hand is empty.";
                return;
            }

            state.Player.Inventory.Add(item);
            state.Message = $"Unequipped: {item.Name}";
        }
    }

    public void UnequipBoth(int playerId)
    {
        lock (_lock)
        {
            if (!_players.TryGetValue(playerId, out PlayerState? state))
                return;

            Player player = state.Player;

            Item? left = player.Equipment.Left;
            Item? right = player.Equipment.Right;

            if (left == null && right == null)
            {
                state.Message = "Both hands are empty.";
                return;
            }

            player.Equipment.Clear();

            if (left != null)
                player.Inventory.Add(left);

            if (right != null && !ReferenceEquals(left, right))
                player.Inventory.Add(right);

            state.Message = "Unequipped both hands.";
        }
    }

    public void DropSelected(int playerId)
    {
        lock (_lock)
        {
            if (!_players.TryGetValue(playerId, out PlayerState? state))
                return;

            Player player = state.Player;
            Cell cell = _room.GetCell(player.Position);

            if (player.Inventory.Count == 0)
            {
                Item? equippedItem = player.Equipment.RemoveLeft();

                if (equippedItem == null)
                    equippedItem = player.Equipment.RemoveRight();

                if (equippedItem == null)
                {
                    state.Message = "Inventory is empty and no item is equipped.";
                    return;
                }

                cell.ItemsOnCell.Add(equippedItem);

                state.Message = $"Dropped equipped item: {equippedItem.Name}. It is under your feet.";
                EventLog.Current.Add($"{player.Name} dropped equipped item: {equippedItem.Name}.");

                return;
            }

            if (state.SelectedIndex < 0 || state.SelectedIndex >= player.Inventory.Count)
            {
                state.Message = "Invalid selection.";
                return;
            }

            Item item = player.Inventory.RemoveAt(state.SelectedIndex);
            cell.ItemsOnCell.Add(item);

            if (state.SelectedIndex >= player.Inventory.Count)
                state.SelectedIndex = Math.Max(0, player.Inventory.Count - 1);

            state.Message = $"Dropped: {item.Name}. It is under your feet.";
            EventLog.Current.Add($"{player.Name} dropped {item.Name}.");
        }
    }

    public void AttackEnemy(int playerId, IAttackStyle style)
    {
        lock (_lock)
        {
            if (!_players.TryGetValue(playerId, out PlayerState? state))
                return;

            Player player = state.Player;
            Cell cell = _room.GetCell(player.Position);

            if (cell.Enemy == null)
            {
                state.Message = "There is no enemy here.";
                return;
            }

            Enemy enemy = cell.Enemy;

            CombatResult result = _combatService.PerformAttack(player, enemy, style);
            state.Message = result.Message;

            if (result.EnemyDefeated)
            {
                enemy.BroadcastDeath();
                _room.UnregisterEnemy(enemy);
                cell.RemoveEnemy();

                state.Message = $"You defeated {enemy.Name}.";
                return;
            }

            if (result.PlayerDefeated || player.IsDead)
            {
                state.IsGameOver = true;
                state.Message = $"GAME OVER: You were defeated by {enemy.Name}. Log file: {_logFilePath}";
                EventLog.Current.Add($"{player.Name} was defeated by {enemy.Name}.");
            }
        }
    }

    public GameSnapshotDto CreateSnapshot()
    {
        lock (_lock)
        {
            var snapshot = new GameSnapshotDto
            {
                Width = _room.Width,
                Height = _room.Height,
                ThemeName = _themeName,
                RecentLogEntries = EventLog.Current.GetRecentEntries().ToList()
            };

            foreach (Position position in _room.GetAllPositions())
            {
                Cell cell = _room.GetCell(position);

                var cellDto = new CellDto
                {
                    X = position.X,
                    Y = position.Y,
                    TileSymbol = cell.Tile.Symbol
                };

                foreach (Item item in cell.ItemsOnCell)
                {
                    cellDto.Items.Add(CreateItemDto(item));
                }

                if (cell.Enemy != null)
                {
                    cellDto.HasEnemy = true;
                    cellDto.EnemyName = cell.Enemy.Name;
                    cellDto.EnemySymbol = cell.Enemy.Symbol;
                    cellDto.EnemyHealth = cell.Enemy.Health;
                    cellDto.EnemyAttack = cell.Enemy.Attack;
                    cellDto.EnemyArmor = cell.Enemy.Armor;
                }

                snapshot.Cells.Add(cellDto);
            }

            foreach (KeyValuePair<int, PlayerState> pair in _players)
            {
                snapshot.Players.Add(CreatePlayerDto(pair.Key, pair.Value));
            }

            return snapshot;
        }
    }

    private Position FindFreeStartPosition()
    {
        foreach (Position position in _room.GetWalkablePositions())
        {
            bool occupied = false;

            foreach (PlayerState state in _players.Values)
            {
                if (state.Player.Position.X == position.X && state.Player.Position.Y == position.Y)
                    occupied = true;
            }

            if (!occupied)
                return position;
        }

        throw new InvalidOperationException("No free starting position.");
    }

    private bool IsOccupiedByOtherPlayer(int currentPlayerId, Position position)
    {
        foreach (KeyValuePair<int, PlayerState> pair in _players)
        {
            if (pair.Key == currentPlayerId)
                continue;

            Player other = pair.Value.Player;

            if (other.Position.X == position.X && other.Position.Y == position.Y)
                return true;
        }

        return false;
    }

    private PlayerDto CreatePlayerDto(int id, PlayerState state)
    {
        Player player = state.Player;

        var dto = new PlayerDto
        {
            Id = id,
            Name = player.Name,
            X = player.Position.X,
            Y = player.Position.Y,
            Health = player.Health,
            IsDead = player.IsDead,
            Coins = player.Wallet.Coins,
            Gold = player.Wallet.Gold,
            Strength = player.EffectiveStrength,
            Dexterity = player.EffectiveDexterity,
            Luck = player.EffectiveLuck,
            Aggression = player.EffectiveAggression,
            Wisdom = player.EffectiveWisdom,
            LeftHand = player.Equipment.Left?.Name ?? "-",
            RightHand = player.Equipment.Right?.Name ?? "-",
            IsInventoryOpen = state.IsInventoryOpen,
            SelectedIndex = state.SelectedIndex,
            Message = state.Message
        };

        foreach (Item item in player.Inventory.Items)
        {
            dto.Inventory.Add(CreateItemDto(item));
        }

        return dto;
    }

    private static ItemDto CreateItemDto(Item item)
    {
        return new ItemDto
        {
            Name = item.Name,
            Description = item.Description,
            Symbol = item.Symbol
        };
    }

    private bool SupportsLoot()
    {
        return _features.HasItems || _features.HasWeapons || _features.HasCurrency;
    }

    private int FindFreePlayerId()
    {
        for (int id = 1; id <= MaxPlayers; id++)
        {
            if (!_players.ContainsKey(id))
                return id;
        }

        throw new InvalidOperationException("No free player id.");
    }
}