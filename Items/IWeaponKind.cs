namespace Rpg;

public interface IWeaponKind{
    int NoiseRange {get;}
    CombatValues GetCombatValues(Player player, Weapon weapon, IAttackStyle style);
}
