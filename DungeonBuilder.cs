namespace Rpg;

public sealed class DungeonBuilder
{
    private readonly int _width;
    private readonly int _height;
    private readonly List<IDungeonBuildStep> _steps = new();
    private readonly Random _random = new();
    private readonly DungeonFeatures _features = new();
    public DungeonFeatures Features => _features;

    public DungeonBuilder(int width, int height)
    {
        _width = width;
        _height = height;
    }

    public DungeonBuilder AddStep(IDungeonBuildStep step)
    {
        if (_steps.Count == 0 && !step.IsStarter)
            throw new InvalidOperationException("First step must be EmptyDungeonStep or FilledDungeonStep.");

        if (_steps.Count > 0 && step.IsStarter)
            throw new InvalidOperationException("Starter step can only be first.");

        _steps.Add(step);
        step.RegisterFeatures(_features);
        return this;
    }

    public Room Build()
    {
        if (_steps.Count == 0)
            throw new InvalidOperationException("Dungeon builder must contain at least one step.");

        var room = new Room(_width, _height);

        foreach (var step in _steps)
            step.Apply(room, _random);

        room.TryPlacePendingItems(_random);
        return room;
    }
}