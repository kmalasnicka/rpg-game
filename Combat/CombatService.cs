namespace Rpg;

public sealed class CombatService{
    public CombatResult PerformAttack(Player player, Enemy enemy, IAttackStyle style){
        Item usedItem = GetCombatItem(player);
        CombatValues values = usedItem.GetCombatValues(player, style);

        int damageToEnemy = Math.Max(0, values.Damage - enemy.Armor);
        enemy.TakeDamage(damageToEnemy);

        if (enemy.IsDead){
            return new CombatResult(damageToEnemy, 0, true, false, $"You used {style.Name} attack and defeated {enemy.Name}.");
        }

        int damageToPlayer = Math.Max(0, enemy.Attack - values.Defense);
        player.TakeDamage(damageToPlayer);
        return new CombatResult(damageToEnemy, damageToPlayer, false, player.IsDead, $"You used {style.Name} attack. You dealt {damageToEnemy} damage and received {damageToPlayer} damage."
        );
    }

    private Item GetCombatItem(Player player){
        if (player.Equipment.Right != null) return player.Equipment.Right;
        if (player.Equipment.Left != null) return player.Equipment.Left;
        return new EmptyHandsItem();
    }
}