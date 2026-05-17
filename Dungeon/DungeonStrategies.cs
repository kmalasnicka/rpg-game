namespace Rpg;

public sealed class DungeonGroundsStrategy : IDungeonStrategy{
    private readonly int _itemsCount;
    private readonly int _weaponsCount;
    private readonly int _coinsCount;
    private readonly int _goldCount;
    private readonly int _enemiesCount;

    public DungeonGroundsStrategy(int itemsCount = 10, int weaponsCount = 6, int coinsCount = 8, int goldCount = 4, int enemiesCount = 5){
        _itemsCount = itemsCount;
        _weaponsCount = weaponsCount;
        _coinsCount = coinsCount;
        _goldCount = goldCount;
        _enemiesCount = enemiesCount;
    }

    public void Build(DungeonBuilder builder){
        builder
            .AddStep(new FilledDungeonStep())
            .AddStep(new AddCentralRoomStep(10, 6))
            .AddStep(new AddPathsStep(28, 8, 20))
            .AddStep(new AddChambersStep(16, 4, 10, 3, 7))
            .AddStep(new EnsureStartAreaStep())
            .AddStep(new AddItemsStep(_itemsCount, new LibraryItemFactory()))
            .AddStep(new AddWeaponsStep(_weaponsCount, new LibraryWeaponFactory()))
            .AddStep(new AddCurrencyStep(_coinsCount, _goldCount))
            .AddStep(new AddEnemiesStep(_enemiesCount, builder.NoiseSubject));
    }
}

public sealed class SimpleRoomStrategy : IDungeonStrategy{
    public void Build(DungeonBuilder builder)
    {
        builder
            .AddStep(new EmptyDungeonStep())
            .AddStep(new ConnectStartToCenterStep())
            .AddStep(new AddCentralRoomStep(12, 8))
            .AddStep(new AddItemsStep(4, new LibraryItemFactory()))
            .AddStep(new AddWeaponsStep(3, new LibraryWeaponFactory()))
            .AddStep(new AddCurrencyStep(3, 2))
            .AddStep(new AddEnemiesStep(5, builder.NoiseSubject));
    }
}

public sealed class EnsureStartAreaStep : IDungeonBuildStep{
    public bool IsStarter => false;
    public void Apply(Room room, Random random){
        for (int y = 0; y <= 2; y++){
            for (int x = 0; x <= 2; x++){
                var position = new Position(x, y);
                if (room.IsInside(position))
                    room.SetTile(position, new EmptyTile());
            }
        }
        DungeonStepHelpers.TryPlacePending(room, random);
    }
    public void RegisterFeatures(DungeonFeatures features) { }
}