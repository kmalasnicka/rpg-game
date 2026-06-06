namespace Rpg;

public sealed class LibraryTheme : IDungeonTheme{
    public string Name => "Library";
    public string IntroMessage => "The smell of old books fills the air.";

    public IDungeonStrategy CreateStrategy(){
        return new LibraryDungeonStrategy(this);
    }

    public IItemFactory CreateItemFactory() => new LibraryItemFactory();
    public IWeaponFactory CreateWeaponFactory() => new LibraryWeaponFactory();
    public Item CreateArtifact() => new BlackWand();
}

public sealed class ForgeTheme : IDungeonTheme{
    public string Name => "Forge";
    public string IntroMessage => "The clang of metal echoes off the walls.";

    public IDungeonStrategy CreateStrategy(){
        return new ForgeDungeonStrategy(this);
    }

    public IItemFactory CreateItemFactory() => new ForgeItemFactory();
    public IWeaponFactory CreateWeaponFactory() => new ForgeWeaponFactory();
    public Item CreateArtifact() => new Blaster();
}

public sealed class TreasuryTheme : IDungeonTheme{
    public string Name => "Treasury";
    public string IntroMessage => "You feel an itch in your wallet.";

    public IDungeonStrategy CreateStrategy(){
        return new TreasuryDungeonStrategy(this);
    }

    public IItemFactory CreateItemFactory() => new TreasuryItemFactory();
    public IWeaponFactory CreateWeaponFactory() => new TreasuryWeaponFactory();
    public Item CreateArtifact() => new LuckyCoinPouch();
}