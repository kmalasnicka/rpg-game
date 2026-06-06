namespace Rpg;

public sealed class Enemy : Fighter, IObserver<NoiseEvent>, IObserver<EnemyDeathEvent> //enemy slucha dzwiekow i informacji o smierci 
{
    private readonly EnemySpecies _species;
    private readonly Subject<NoiseEvent> _noiseSubject;
    public string Name {get;}
    public char Symbol { get; }
    public int Attack { get; private set; }
    public int Armor { get; private set; }
    public Position Position { get; private set; }

    public Enemy(string name, char symbol, int health, int attack, int armor, Position position, EnemySpecies species, Subject<NoiseEvent> noiseSubject) : base(health){
        Name = name;
        Symbol = symbol;
        Attack = attack;
        Armor = armor;
        Position = position;
        _species = species;
        _noiseSubject = noiseSubject;
        _species.Join(this);
        _noiseSubject.Attach(this);
    }

    public void SetPosition(Position position){
        Position = position;
    }

    public void ChangeStats(int attackChange, int armorChange){
        Attack = Math.Max(1, Attack + attackChange);
        Armor = Math.Max(0, Armor + armorChange);
    }

    public void BroadcastDeath(){
        _species.BroadcastDeath(this);
    }

    public void RemoveSubscriptions(){
        _species.Leave(this);
        _noiseSubject.Detach(this);
    }

    public void Notify(NoiseEvent value){
        int? distance = value.Room.FindDistance(value.Source, Position, value.Range);
        if (distance == null) return;

        EventLog.Current.Add($"{Name} at {Position} heard noise from {value.Source} at distance {distance.Value}.");

        var next = value.Room.FindStepTowards(Position, value.Source, value.Range);
        if (next == null) return;
        bool nextIsSoundSource = next.Value.X == value.Source.X && next.Value.Y == value.Source.Y;
        //enemy idzie w strone dzwieku 
        if (value.Room.CanEnter(next.Value) && (!value.Room.GetCell(next.Value).HasEnemy() || nextIsSoundSource))
        {
            value.Room.GetCell(Position).RemoveEnemy();
            value.Room.GetCell(next.Value).SetEnemy(this);
            Position = next.Value;
        }
    }

    public void Notify(EnemyDeathEvent value){
        if (value.DeadEnemy == this) return;
        if (IsDead) return;
        _species.Reaction.ReactToDeath(this);
    }
}