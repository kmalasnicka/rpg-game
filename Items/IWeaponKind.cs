namespace Rpg;

public interface IWeaponKind{
    CombatValues GetCombatValues(Player player, Weapon weapon, IAttackStyle style);
}
