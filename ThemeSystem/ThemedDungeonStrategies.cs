namespace Rpg;

public sealed class LibraryDungeonStrategy : IDungeonStrategy{
    private readonly IDungeonTheme _theme;

    public LibraryDungeonStrategy(IDungeonTheme theme){
        _theme = theme;
    }

    public void Build(DungeonBuilder builder){
        builder
            .AddStep(new FilledDungeonStep())
            .AddStep(new AddPathsStep(35, 8, 22))
            .AddStep(new AddChambersStep(8, 3, 6, 3, 5))
            .AddStep(new EnsureStartAreaStep())
            .AddStep(new ConnectStartToCenterStep())
            .AddStep(new AddItemsStep(12, _theme.CreateItemFactory()))
            .AddStep(new AddWeaponsStep(7, _theme.CreateWeaponFactory()))
            .AddStep(new AddArtifactStep(_theme.CreateArtifact()))
            .AddStep(new AddEnemiesStep(6, builder.NoiseSubject));
    }
}

public sealed class ForgeDungeonStrategy : IDungeonStrategy{
    private readonly IDungeonTheme _theme;

    public ForgeDungeonStrategy(IDungeonTheme theme){
        _theme = theme;
    }

    public void Build(DungeonBuilder builder){
        builder
            .AddStep(new FilledDungeonStep())
            .AddStep(new AddCentralRoomStep(14, 8))
            .AddStep(new AddChambersStep(18, 4, 10, 3, 7))
            .AddStep(new AddPathsStep(18, 6, 16))
            .AddStep(new EnsureStartAreaStep())
            .AddStep(new ConnectStartToCenterStep())
            .AddStep(new AddItemsStep(15, _theme.CreateItemFactory()))
            .AddStep(new AddWeaponsStep(7, _theme.CreateWeaponFactory()))
            .AddStep(new AddArtifactStep(_theme.CreateArtifact()))
            .AddStep(new AddEnemiesStep(7, builder.NoiseSubject));
    }
}

public sealed class TreasuryDungeonStrategy : IDungeonStrategy{
    private readonly IDungeonTheme _theme;

    public TreasuryDungeonStrategy(IDungeonTheme theme){
        _theme = theme;
    }

    public void Build(DungeonBuilder builder){
        builder
            .AddStep(new FilledDungeonStep())
            .AddStep(new AddCentralRoomStep(20, 10))
            .AddStep(new AddPathsStep(25, 8, 20))
            .AddStep(new EnsureStartAreaStep())
            .AddStep(new ConnectStartToCenterStep())
            .AddStep(new AddItemsStep(25, _theme.CreateItemFactory()))
            .AddStep(new AddArtifactStep(_theme.CreateArtifact()))
            .AddStep(new AddEnemiesStep(6, builder.NoiseSubject));
    }
}