namespace Rpg;

public interface IDungeonTheme{
    string Name { get; }
    string IntroMessage { get; }

    IDungeonStrategy CreateStrategy();
    IItemFactory CreateItemFactory();
    IWeaponFactory CreateWeaponFactory();
    //IEnemyFactory CreateEnemyFactory();
    Item CreateArtifact();
}