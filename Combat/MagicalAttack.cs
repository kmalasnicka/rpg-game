namespace Rpg;

public sealed class MagicalAttack : IAttackStyle{
    public string Name => "Magical";

    public CombatValues UseWithHeavy(Player player, Weapon weapon){
        int damage = 1;
        int defense = player.EffectiveLuck;
        return new CombatValues(damage, defense);
    }

    public CombatValues UseWithLight(Player player, Weapon weapon){
        int damage = 1;
        int defense = player.EffectiveLuck;
        return new CombatValues(damage, defense);
    }

    public CombatValues UseWithMagical(Player player, Weapon weapon){
        int damage = weapon.BaseDamage + player.EffectiveWisdom + weapon.GetDamageModifier();
        int defense = player.EffectiveWisdom * 2;
        return new CombatValues(damage, defense);
    }

    public CombatValues UseWithoutWeapon(Player player) => new CombatValues(0, player.EffectiveLuck);
}