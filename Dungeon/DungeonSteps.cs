namespace Rpg;
using System.Linq;

internal static class DungeonStepHelpers
{
    public static void TryPlacePending(Room room, Random random)
    {
        room.TryPlacePendingItems(random);
    }

    public static Item CreateRandomItem(Random random)
    {
        return random.Next(3) switch
        {
            0 => new Bottle(),
            1 => new Feather(),
            _ => new ScrapMetal()
        };
    }

    public static Weapon CreateRandomWeapon(Random random)
    {
        return random.Next(4) switch
        {
            0 => new Dagger(),
            1 => new Sword(),
            2 => new Axe(),
            _ => new Wand()
        };
    }

    public static Weapon ApplyRandomWeaponModifiers(Weapon weapon, Random random)
    {
        if (random.NextDouble() < 0.35)
            weapon = new StrongWeaponModifier(weapon);

        if (random.NextDouble() < 0.35)
            weapon = new UnluckyWeaponModifier(weapon);

        return weapon;
    }

    public static Item ApplyRandomItemModifiers(Item item, Random random)
    {
        if (random.NextDouble() < 0.35)
            item = new UnluckyItemModifier(item);

        return item;
    }
}

public sealed class EmptyDungeonStep : IDungeonBuildStep
{
    public bool IsStarter => true;

    public void Apply(Room room, Random random)
    {
        foreach (var position in room.GetAllPositions())
            room.SetTile(position, new EmptyTile());

        DungeonStepHelpers.TryPlacePending(room, random);
    }
    public void RegisterFeatures(DungeonFeatures features) { }
}

public sealed class FilledDungeonStep : IDungeonBuildStep
{
    public bool IsStarter => true;

    public void Apply(Room room, Random random)
    {
        for (int y = 0; y < room.Height; y++)
        {
            for (int x = 0; x < room.Width; x++)
                room.SetTile(new Position(x, y), new WallTile());
        }
    }
    public void RegisterFeatures(DungeonFeatures features) { }
}

public sealed class AddCentralRoomStep : IDungeonBuildStep
{
    private readonly int _roomWidth;
    private readonly int _roomHeight;

    public AddCentralRoomStep(int roomWidth, int roomHeight)
    {
        _roomWidth = roomWidth;
        _roomHeight = roomHeight;
    }

    public bool IsStarter => false;

    public void Apply(Room room, Random random)
    {
        int carvedWidth = Math.Min(_roomWidth, room.Width);
        int carvedHeight = Math.Min(_roomHeight, room.Height);

        int startX = Math.Max(0, room.Width / 2 - carvedWidth / 2);
        int startY = Math.Max(0, room.Height / 2 - carvedHeight / 2);

        for (int y = 0; y < carvedHeight; y++)
        {
            for (int x = 0; x < carvedWidth; x++)
                room.SetTile(new Position(startX + x, startY + y), new EmptyTile());
        }

        DungeonStepHelpers.TryPlacePending(room, random);
    }
    public void RegisterFeatures(DungeonFeatures features) { }
}

public sealed class AddChambersStep : IDungeonBuildStep
{
    private readonly int _count;
    private readonly int _minWidth;
    private readonly int _maxWidth;
    private readonly int _minHeight;
    private readonly int _maxHeight;

    public AddChambersStep(
        int count = 6,
        int minWidth = 3,
        int maxWidth = 7,
        int minHeight = 3,
        int maxHeight = 5)
    {
        _count = count;
        _minWidth = minWidth;
        _maxWidth = maxWidth;
        _minHeight = minHeight;
        _maxHeight = maxHeight;
    }

    public bool IsStarter => false;

    public void Apply(Room room, Random random)
    {
        for (int i = 0; i < _count; i++)
        {
            var chamberWidth = random.Next(_minWidth, _maxWidth + 1);
            var chamberHeight = random.Next(_minHeight, _maxHeight + 1);

            chamberWidth = Math.Min(chamberWidth, room.Width);
            chamberHeight = Math.Min(chamberHeight, room.Height);

            var startX = random.Next(0, Math.Max(1, room.Width - chamberWidth + 1));
            var startY = random.Next(0, Math.Max(1, room.Height - chamberHeight + 1));

            for (int y = startY; y < startY + chamberHeight; y++)
            {
                for (int x = startX; x < startX + chamberWidth; x++)
                    room.SetTile(new Position(x, y), new EmptyTile());
            }
        }

        DungeonStepHelpers.TryPlacePending(room, random);
    }
    public void RegisterFeatures(DungeonFeatures features) { }
}

public sealed class AddPathsStep : IDungeonBuildStep
{
    private readonly int _count;
    private readonly int _minLength;
    private readonly int _maxLength;

    public AddPathsStep(int count = 12, int minLength = 5, int maxLength = 14)
    {
        _count = count;
        _minLength = minLength;
        _maxLength = maxLength;
    }

    public bool IsStarter => false;

    public void Apply(Room room, Random random)
    {
        for (int i = 0; i < _count; i++)
        {
            var start = new Position(random.Next(room.Width), random.Next(room.Height));
            var horizontal = random.Next(2) == 0;
            var length = random.Next(_minLength, _maxLength + 1);
            var direction = random.Next(2) == 0 ? -1 : 1;

            for (int step = 0; step < length; step++)
            {
                var x = start.X + (horizontal ? step * direction : 0);
                var y = start.Y + (horizontal ? 0 : step * direction);
                var position = new Position(x, y);

                if (!room.IsInside(position))
                    break;

                room.SetTile(position, new EmptyTile());
            }
        }

        DungeonStepHelpers.TryPlacePending(room, random);
    }
    public void RegisterFeatures(DungeonFeatures features) { }
}

public sealed class AddItemsStep : IDungeonBuildStep
{
    private readonly int _count;
    private readonly IItemFactory _factory;

    public AddItemsStep(int count, IItemFactory factory)
    {
        _count = count;
        _factory = factory;
    }

    public bool IsStarter => false;

    public void Apply(Room room, Random random)
    {
        for (int i = 0; i < _count; i++)
        {
            Item item = _factory.Create(random);
            item = DungeonStepHelpers.ApplyRandomItemModifiers(item, random);
            room.QueueItem(item);
        }

        DungeonStepHelpers.TryPlacePending(room, random);
    }

    public void RegisterFeatures(DungeonFeatures features)
    {
        features.HasItems = true;
    }
}

public sealed class AddWeaponsStep : IDungeonBuildStep
{
    private readonly int _count;
    private readonly IWeaponFactory _factory;

    public AddWeaponsStep(int count, IWeaponFactory factory)
    {
        _count = count;
        _factory = factory;
    }

    public bool IsStarter => false;

    public void Apply(Room room, Random random)
    {
        for (int i = 0; i < _count; i++)
        {
            Weapon weapon = _factory.Create(random);
            weapon = DungeonStepHelpers.ApplyRandomWeaponModifiers(weapon, random);
            room.QueueItem(weapon);
        }

        DungeonStepHelpers.TryPlacePending(room, random);
    }

    public void RegisterFeatures(DungeonFeatures features)
    {
        features.HasWeapons = true;
    }
}

public sealed class AddArtifactStep : IDungeonBuildStep
{
    private readonly Item _artifact;

    public AddArtifactStep(Item artifact)
    {
        _artifact = artifact;
    }

    public bool IsStarter => false;

    public void Apply(Room room, Random random)
    {
        var pos = room.FindRandomEmptyWalkablePositionWithoutEnemy(random);

        if (pos != null)
        {
            room.PlaceItem(pos.Value, _artifact);
        }
        DungeonStepHelpers.TryPlacePending(room, random);
        EventLog.Current.Add($"Artifact placed in dungeon: {_artifact.Name}.");
    }

    public void RegisterFeatures(DungeonFeatures features)
    {
        features.HasItems = true;
        features.HasWeapons = true;
    }
}

public sealed class AddCurrencyStep : IDungeonBuildStep
{
    private readonly int _coinsCount;
    private readonly int _goldCount;

    public AddCurrencyStep(int coinsCount, int goldCount)
    {
        _coinsCount = coinsCount;
        _goldCount = goldCount;
    }

    public bool IsStarter => false;

    public void Apply(Room room, Random random)
    {
        for (int i = 0; i < _coinsCount; i++)
            room.QueueItem(new Coin());

        for (int i = 0; i < _goldCount; i++)
            room.QueueItem(new Gold());

        DungeonStepHelpers.TryPlacePending(room, random);
    }
    public void RegisterFeatures(DungeonFeatures features)
    {
        features.HasCurrency = true;
    }
}

public sealed class AddEnemiesStep : IDungeonBuildStep
{
    private readonly int _count;
    private readonly IEnemyFactory _factory;

    public AddEnemiesStep(int count, IEnemyFactory factory)
    {
        _count = count;
        _factory = factory;
    }

    public bool IsStarter => false;

    public void Apply(Room room, Random random)
    {
        int created = 0;

        for (int i = 0; i < _count; i++)
        {
            var positions = room.GetWalkablePositions()
                .Where(position => !room.GetCell(position).HasEnemy())
                .Where(position => !(position.X <= 2 && position.Y <= 2))
                .ToList();

            if (positions.Count == 0) break;

            var pos = positions[random.Next(positions.Count)];
            var enemy = _factory.Create(random);
            room.GetCell(pos).SetEnemy(enemy);

            created++;
        }

        if (created > 0)
            EventLog.Current.Add($"{created} enemies appeared in the dungeon.");
    }

    public void RegisterFeatures(DungeonFeatures features){
        features.HasEnemies = true;
    }
}