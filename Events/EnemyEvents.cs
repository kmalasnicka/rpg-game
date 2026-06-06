namespace Rpg;

public sealed class NoiseEvent{ //halas z konkretnego pola
    public Position Source { get; }
    public int Range { get; }
    public Room Room { get; }

    public NoiseEvent(Position source, int range, Room room){ 
        Source = source;
        Range = range;
        Room = room;
    }
}

public sealed class EnemyDeathEvent{ //enemy zginal
    public Enemy DeadEnemy { get; }

    public EnemyDeathEvent(Enemy deadEnemy){
        DeadEnemy = deadEnemy;
    }
}