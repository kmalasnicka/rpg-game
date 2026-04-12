namespace Rpg;

public sealed class NormalAttack : IAttackStyle
{
    public string Name => "Normal";

    public CombatValues UseWithHeavy(Player player, Weapon weapon){
        int damage = weapon.BaseDamage + player.EffectiveStrength + player.EffectiveAggression + weapon.GetDamageModifier();
        int defense = player.EffectiveStrength + player.EffectiveLuck;
        return new CombatValues(damage, defense);
    }

    public CombatValues UseWithLight(Player player, Weapon weapon){
        int damage = weapon.BaseDamage + player.EffectiveDexterity + player.EffectiveLuck + weapon.GetDamageModifier();
        int defense = player.EffectiveDexterity + player.EffectiveLuck;
        return new CombatValues(damage, defense);
    }

    public CombatValues UseWithMagical(Player player, Weapon weapon){
        int damage = 1;
        int defense = player.EffectiveDexterity + player.EffectiveLuck;
        return new CombatValues(damage, defense);
    }

    public CombatValues UseWithoutWeapon(Player player) => new CombatValues(0, player.EffectiveDexterity);
}