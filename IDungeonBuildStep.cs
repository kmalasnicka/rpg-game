namespace Rpg;

public interface IDungeonBuildStep
{
    bool IsStarter { get; } //czy dany krok moze byc poczatkiem budowy
    void Apply(Room room, Random random);
    void RegisterFeatures(DungeonFeatures features);
}

