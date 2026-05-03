namespace Rpg;

public interface IEnemyFactory{
    Enemy Create(Random random);
}

public interface IItemFactory{
    Item Create(Random random);
}

public interface IWeaponFactory{
    Weapon Create(Random random);
}