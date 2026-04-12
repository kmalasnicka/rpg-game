namespace Rpg;

public sealed class HeavyWeaponKind : IWeaponKind{
    public CombatValues GetCombatValues(Player player, Weapon weapon, IAttackStyle style) => style.UseWithHeavy(player, weapon);
}

public sealed class LightWeaponKind : IWeaponKind{
    public CombatValues GetCombatValues(Player player, Weapon weapon, IAttackStyle style) => style.UseWithLight(player, weapon);
}

public sealed class MagicalWeaponKind : IWeaponKind{
    public CombatValues GetCombatValues(Player player, Weapon weapon, IAttackStyle style) => style.UseWithMagical(player, weapon);
}