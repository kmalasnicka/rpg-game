namespace Rpg;

public interface IDungeonTheme{
    string Name { get; }
    string IntroMessage { get; }

    IDungeonStrategy CreateStrategy();
    IItemFactory CreateItemFactory();
    IWeaponFactory CreateWeaponFactory();
    Item CreateArtifact();
}