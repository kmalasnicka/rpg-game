namespace Rpg;

public sealed class DungeonGroundsStrategy : IDungeonStrategy{
    private readonly int _itemsCount;
    private readonly int _weaponsCount;
    private readonly int _coinsCount;
    private readonly int _goldCount;

    public DungeonGroundsStrategy(int itemsCount = 10, int weaponsCount = 6, int coinsCount = 8, int goldCount = 4){
        _itemsCount = itemsCount;
        _weaponsCount = weaponsCount;
        _coinsCount = coinsCount;
        _goldCount = goldCount;
    }

    public void Build(DungeonBuilder builder){
        builder
            .AddStep(new FilledDungeonStep())
            .AddStep(new AddCentralRoomStep(10, 6))
            .AddStep(new AddPathsStep(28, 8, 20))
            .AddStep(new AddChambersStep(16, 4, 10, 3, 7))
            .AddStep(new EnsureStartAreaStep())
            .AddStep(new AddItemsStep(_itemsCount))
            .AddStep(new AddWeaponsStep(_weaponsCount))
            .AddStep(new AddCurrencyStep(_coinsCount, _goldCount));
    }
}

public sealed class SimpleRoomStrategy : IDungeonStrategy{
    public void Build(DungeonBuilder builder){
        builder
            .AddStep(new EmptyDungeonStep())
            .AddStep(new AddCentralRoomStep(12, 8))
            .AddStep(new AddItemsStep(4))
            .AddStep(new AddWeaponsStep(3))
            .AddStep(new AddCurrencyStep(3, 2));
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
}