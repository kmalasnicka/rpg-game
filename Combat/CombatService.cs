namespace Rpg;

public sealed class CombatService{
    public CombatResult PerformAttack(Player player, Enemy enemy, IAttackStyle style){
        Item usedItem = GetCombatItem(player);
        CombatValues values = usedItem.GetCombatValues(player, style);

        int damageToEnemy = Math.Max(0, values.Damage - enemy.Armor);
        EventLog.Current.Add($"{player.Name} used {style.Name} attack with {usedItem.Name} and dealt {damageToEnemy} damage to {enemy.Name}.");
        
        enemy.TakeDamage(damageToEnemy);

        if (enemy.IsDead){
            EventLog.Current.Add($"{enemy.Name} was defeated.");
            return new CombatResult(damageToEnemy, 0, true, false, $"You used {style.Name} attack and defeated {enemy.Name}.");
        }

        int damageToPlayer = Math.Max(0, enemy.Attack - values.Defense);
        EventLog.Current.Add($"{enemy.Name} attacked {player.Name} and dealt {damageToPlayer} damage.");
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