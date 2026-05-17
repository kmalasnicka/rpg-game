namespace Rpg;

public abstract class WeaponModifier : Weapon{
    protected Weapon InnerWeapon { get; }
    protected WeaponModifier(Weapon innerWeapon){
        InnerWeapon = innerWeapon;
    }

    public override string Name => InnerWeapon.Name;
    public override string Description => $"Damage: {BaseDamage + GetDamageModifier()}";    public override char Symbol => InnerWeapon.Symbol;
    public override int BaseDamage => InnerWeapon.BaseDamage;
    public override bool IsTwoHanded => InnerWeapon.IsTwoHanded;
    public override IWeaponKind WeaponKind => InnerWeapon.WeaponKind;

    public override CombatValues GetCombatValues(Player player, IAttackStyle style) => WeaponKind.GetCombatValues(player, this, style);

    public override int GetLuckModifier() => InnerWeapon.GetLuckModifier();
    public override int GetDamageModifier() => InnerWeapon.GetDamageModifier();
    public override int GetStrengthModifier() => InnerWeapon.GetStrengthModifier();
    public override int GetDexterityModifier() => InnerWeapon.GetDexterityModifier();
    public override int GetAggressionModifier() => InnerWeapon.GetAggressionModifier();
    public override int GetWisdomModifier() => InnerWeapon.GetWisdomModifier();
    public override int NoiseRange => InnerWeapon.NoiseRange;
    
    public override bool TryEquip(Player player, bool targetLeftHand){
        var left = player.Equipment.Left;
        var right = player.Equipment.Right;

        if (IsTwoHanded){
            if (left != null) player.Inventory.Add(left);
            if (right != null && !ReferenceEquals(left, right)) player.Inventory.Add(right);

            player.Equipment.Clear();
            player.Equipment.SetLeft(this);
            player.Equipment.SetRight(this);
            return true;
        }

        if (targetLeftHand){
            if (left != null && ReferenceEquals(left, right)){
                player.Inventory.Add(left);
                player.Equipment.Clear();
            }
            else{
                if (left != null) player.Inventory.Add(left);
                player.Equipment.SetLeft(null);
            }

            player.Equipment.SetLeft(this);
        }
        else{
            if (right != null && ReferenceEquals(left, right)){
                player.Inventory.Add(right);
                player.Equipment.Clear();
            }
            else{
                if (right != null) player.Inventory.Add(right);
                player.Equipment.SetRight(null);
            }

            player.Equipment.SetRight(this);
        }

        return true;
    }

    public override void EquipLeft(Player player){
        if (IsTwoHanded){
            player.Equipment.Clear();
            player.Equipment.SetLeft(this);
            player.Equipment.SetRight(this);
            return;
        }

        player.Equipment.SetLeft(this);
    }

    public override void EquipRight(Player player){
        if (IsTwoHanded){
            player.Equipment.Clear();
            player.Equipment.SetLeft(this);
            player.Equipment.SetRight(this);
            return;
        }

        player.Equipment.SetRight(this);
    }
}

public sealed class StrongWeaponModifier : WeaponModifier{
    public StrongWeaponModifier(Weapon innerWeapon) : base(innerWeapon) { }

    public override string Name => $"{InnerWeapon.Name} (Strong)";
    public override int GetDamageModifier() => InnerWeapon.GetDamageModifier() + 5;
}

public sealed class UnluckyWeaponModifier : WeaponModifier{
    public UnluckyWeaponModifier(Weapon innerWeapon) : base(innerWeapon) { }

    public override string Name => $"{InnerWeapon.Name} (Unlucky)";
    public override int GetLuckModifier() => InnerWeapon.GetLuckModifier() - 5;
}