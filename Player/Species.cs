namespace Rpg;

public interface ISpeciesReaction{ //rozne gatunki moga miec rozna reakcje na smierc innego enemy (strategy pattern)
    string SpeciesName { get; }
    void ReactToDeath(Enemy enemy); 
}

public sealed class CowardlySpeciesReaction : ISpeciesReaction{
    public string SpeciesName { get; }

    public CowardlySpeciesReaction(string speciesName){
        SpeciesName = speciesName;
    }

    public void ReactToDeath(Enemy enemy){
        enemy.ChangeStats(-2, -1); //enemy slabnie
        EventLog.Current.Add($"{enemy.Name} became weaker after hearing about death in species {SpeciesName}.");
    }
}

public sealed class AggressiveSpeciesReaction : ISpeciesReaction{
    public string SpeciesName { get; }

    public AggressiveSpeciesReaction(string speciesName){
        SpeciesName = speciesName;
    }

    public void ReactToDeath(Enemy enemy){
        enemy.ChangeStats(2, 1);//enemy staje sie silniejszy
        EventLog.Current.Add($"{enemy.Name} became stronger after hearing about death in species {SpeciesName}.");
    }
}

public sealed class EnemySpecies{
    private readonly Subject<EnemyDeathEvent> _deathSubject = new(); //obiekt powiadamiajacy obserwatorow o smierci enemy
    public ISpeciesReaction Reaction { get; } //kazdy gatunek ma swoja reakcje 

    public EnemySpecies(ISpeciesReaction reaction){
        Reaction = reaction;
    }

    public void Join(Enemy enemy){ 
        _deathSubject.Attach(enemy); //dodajemy enemy do obserwatorow 
    }

    public void Leave(Enemy enemy){ 
        _deathSubject.Detach(enemy); //usuwamy enemy z obserwatorow 
    }

    public void BroadcastDeath(Enemy enemy){
        _deathSubject.NotifyAll(new EnemyDeathEvent(enemy)); //wysyamy event smierci do wszystkich obserwatorow 
    }
}