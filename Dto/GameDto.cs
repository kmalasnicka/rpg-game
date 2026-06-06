namespace Rpg;

public sealed class GameSnapshotDto
{
    public int Width { get; set; }
    public int Height { get; set; }
    public string ThemeName { get; set; } = "";
    public List<CellDto> Cells { get; set; } = new();
    public List<PlayerDto> Players { get; set; } = new();
    public List<string> RecentLogEntries { get; set; } = new();
}

public sealed class CellDto
{
    public int X { get; set; }
    public int Y { get; set; }

    public char TileSymbol { get; set; }

    public List<ItemDto> Items { get; set; } = new();

    public bool HasEnemy { get; set; }
    public string EnemyName { get; set; } = "";
    public char EnemySymbol { get; set; }
    public int EnemyHealth { get; set; }
    public int EnemyAttack { get; set; }
    public int EnemyArmor { get; set; }
}

public sealed class ItemDto
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public char Symbol { get; set; }
}

public sealed class PlayerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public int X { get; set; }
    public int Y { get; set; }

    public int Health { get; set; }
    public bool IsDead { get; set; }

    public int Coins { get; set; }
    public int Gold { get; set; }

    public int Strength { get; set; }
    public int Dexterity { get; set; }
    public int Luck { get; set; }
    public int Aggression { get; set; }
    public int Wisdom { get; set; }

    public string LeftHand { get; set; } = "-";
    public string RightHand { get; set; } = "-";

    public bool IsInventoryOpen { get; set; }
    public int SelectedIndex { get; set; }
    public string Message { get; set; } = "";

    public List<ItemDto> Inventory { get; set; } = new();
}