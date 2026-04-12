namespace Rpg;

public sealed class CombatResult{
    public int DamageToEnemy{get;}
    public int DamageToPlayer{get;}
    public bool EnemyDefeated {get;}
    public bool PlayerDefeated {get;}
    public string Message {get;}

    public CombatResult(int damageToEnemy, int damageToPlayer, bool enemyDefeated, bool playerDefeated, string message){
        DamageToEnemy = damageToEnemy;
        DamageToPlayer = damageToPlayer;
        EnemyDefeated = enemyDefeated;
        PlayerDefeated = playerDefeated;
        Message = message;
    }
}
