namespace Rpg;

public sealed class HeavyWeaponKind : IWeaponKind{
    public int NoiseRange => 8;
    public CombatValues GetCombatValues(Player player, Weapon weapon, IAttackStyle style) => style.UseWithHeavy(player, weapon);
}

public sealed class LightWeaponKind : IWeaponKind{
    public int NoiseRange => 1;
    public CombatValues GetCombatValues(Player player, Weapon weapon, IAttackStyle style) => style.UseWithLight(player, weapon);
}

public sealed class MagicalWeaponKind : IWeaponKind{
    public int NoiseRange => 5;
    public CombatValues GetCombatValues(Player player, Weapon weapon, IAttackStyle style) => style.UseWithMagical(player, weapon);
}