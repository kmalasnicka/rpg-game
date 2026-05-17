namespace Rpg;


public abstract class Weapon : Item, IEquipable{
    public abstract int BaseDamage { get; }
    public override string Description => $"Damage: {BaseDamage}";
    public abstract bool IsTwoHanded { get; }
    public abstract IWeaponKind WeaponKind { get; }

    public abstract void EquipLeft(Player player);
    public abstract void EquipRight(Player player);
    public override CombatValues GetCombatValues(Player player, IAttackStyle style) => WeaponKind.GetCombatValues(player, this, style);
    public override bool AffectsPlayerWhileInInventory => false;
    public override int NoiseRange => WeaponKind.NoiseRange;
}

public abstract class OneHandWeapon : Weapon{
    public override bool TryEquip(Player player, bool targetLeftHand){
        var left = player.Equipment.Left;
        var right = player.Equipment.Right;

        if(targetLeftHand){ //chcemy wlozyc do lewej reki
            if(left != null && ReferenceEquals(left, right)){ //bron dwureczna
                player.Inventory.Add(left); 
                player.Equipment.Clear(); //czyscimy obie rece
            }else{ 
                if(left != null) player.Inventory.Add(left);
                player.Equipment.SetLeft(null); 
            }
            player.Equipment.SetLeft(this);//ustawiamy bron w rece
        }else{//chcemy wlozyc do prawej reki
            if(right != null && ReferenceEquals(left, right)){
                player.Inventory.Add(right);
                player.Equipment.Clear();
            }else{
                if(right != null) player.Inventory.Add(right);
                player.Equipment.SetRight(null);
            }
            player.Equipment.SetRight(this);
        }
        return true;
    }

    public override void EquipLeft(Player player) => player.Equipment.SetLeft(this);
    public override void EquipRight(Player player) => player.Equipment.SetRight(this);
}

public abstract class TwoHandWeapon : Weapon{
    public override bool TryEquip(Player player, bool targetLeftHand){
        var left = player.Equipment.Left;
        var right = player.Equipment.Right;
        if(left != null) player.Inventory.Add(left);
        if(right != null && !ReferenceEquals(left, right)) player.Inventory.Add(right); //jesli cos innego niz w lewej to dodajemy do inventory
        player.Equipment.Clear();
        player.Equipment.SetLeft(this);
        player.Equipment.SetRight(this);
        return true;
    }

    public override void EquipLeft(Player player){
        player.Equipment.Clear();
        player.Equipment.SetLeft(this);
        player.Equipment.SetRight(this);
    }

    public override void EquipRight(Player player){
        player.Equipment.Clear();
        player.Equipment.SetLeft(this);
        player.Equipment.SetRight(this);
    }
}

public sealed class Dagger : OneHandWeapon{
    private static readonly IWeaponKind _kind = new LightWeaponKind();
    public override int BaseDamage => 2;
    public override char Symbol => 'd';
    public override string Name => "Dagger";
    public override bool IsTwoHanded => false;
    public override IWeaponKind WeaponKind => _kind;
}

public sealed class Sword : OneHandWeapon{
    private static readonly IWeaponKind _kind = new LightWeaponKind();
    public override int BaseDamage => 5;
    public override char Symbol => 's';
    public override string Name => "Sword";
    public override bool IsTwoHanded => false;
    public override IWeaponKind WeaponKind => _kind;
}

public sealed class Axe : TwoHandWeapon{
    private static readonly IWeaponKind _kind = new HeavyWeaponKind();
    public override int BaseDamage => 10;
    public override char Symbol => 'a';
    public override string Name => "Axe";
    public override bool IsTwoHanded => true;
    public override IWeaponKind WeaponKind => _kind;
}

public sealed class Wand : TwoHandWeapon{
    private static readonly IWeaponKind _kind = new MagicalWeaponKind();
    public override int BaseDamage => 6;
    public override char Symbol => 'w';
    public override string Name => "Wand";
    public override bool IsTwoHanded => true;
    public override IWeaponKind WeaponKind => _kind;
}
