namespace Rpg;

public sealed class LibraryItemFactory : IItemFactory{
    public Item Create(Random random){
        return random.Next(3) switch{
            0 => new OldBook(),
            1 => new Feather(),
            _ => new Bottle()
        };
    }
}

public sealed class LibraryWeaponFactory : IWeaponFactory{
    public Weapon Create(Random random){
        return random.Next(3) switch{
            0 => new Wand(),
            1 => new Dagger(),
            _ => new Sword()
        };
    }
}

public sealed class ForgeItemFactory : IItemFactory{
    public Item Create(Random random){
        return random.Next(3) switch{
            0 => new MetalFragment(),
            1 => new ScrapMetal(),
            _ => new Bottle()
        };
    }
}

public sealed class ForgeWeaponFactory : IWeaponFactory{
    public Weapon Create(Random random){
        return random.Next(3) switch{
            0 => new Axe(),
            1 => new Sword(),
            _ => new Dagger()
        };
    }
}

public sealed class TreasuryItemFactory : IItemFactory{
    public Item Create(Random random){
        return random.Next(2) == 0 ? new Coin() : new Gold();
    }
}

public sealed class TreasuryWeaponFactory : IWeaponFactory{
    public Weapon Create(Random random){
        return new LuckyCoinPouch();
    }
}
