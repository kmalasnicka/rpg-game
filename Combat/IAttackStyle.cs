namespace Rpg;

public readonly record struct CombatValues(int Damage, int Defense);

public interface IAttackStyle
{
    string Name {get;}
    CombatValues UseWithHeavy(Player player, Weapon weapon);
    CombatValues UseWithLight(Player player, Weapon weapon);
    CombatValues UseWithMagical(Player player, Weapon weapon);
    CombatValues UseWithoutWeapon(Player player);
}
