namespace Rpg;

public sealed class StealthAttack : IAttackStyle{
    public string Name => "Stealth";

    public CombatValues UseWithHeavy(Player player, Weapon weapon){
        int baseDamage = weapon.BaseDamage + player.EffectiveStrength + player.EffectiveAggression + weapon.GetDamageModifier();
        int damage = baseDamage / 2;
        int defense = player.EffectiveStrength;
        return new CombatValues(damage, defense);
    }

    public CombatValues UseWithLight(Player player, Weapon weapon){
        int baseDamage = weapon.BaseDamage + player.EffectiveDexterity + player.EffectiveLuck + weapon.GetDamageModifier();
        int damage = baseDamage * 2;
        int defense = player.EffectiveDexterity;
        return new CombatValues(damage, defense);
    }

    public CombatValues UseWithMagical(Player player, Weapon weapon) => new CombatValues(1, 0);
    public CombatValues UseWithoutWeapon(Player player) => new CombatValues(0, 0);
}