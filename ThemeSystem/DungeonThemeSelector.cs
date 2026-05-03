namespace Rpg;

public sealed class DungeonThemeSelector{
    private readonly Dictionary<string, Func<IDungeonTheme>> _themes =
        new(StringComparer.OrdinalIgnoreCase){
            ["library"] = () => new LibraryTheme(),
            ["forge"] = () => new ForgeTheme(),
            ["treasury"] = () => new TreasuryTheme()
        };

    public IDungeonTheme Select(string name){
        if (_themes.TryGetValue(name, out var createTheme))
            return createTheme();
        return new LibraryTheme();
    }
}