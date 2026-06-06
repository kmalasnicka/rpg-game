using System.Text;

namespace Rpg;

public sealed class ConsoleGameView
{
    private const int FixedWindowWidth = 120;
    private const int FixedWindowHeight = 40;

    public void Initialize()
    {
        Console.Clear();
        Console.CursorVisible = false;
    }

    public void Shutdown()
    {
        Console.CursorVisible = true;
    }

    public void Render(GameSnapshotDto snapshot, int localPlayerId)
    {
        int windowWidth = Math.Min(FixedWindowWidth, Console.WindowWidth);
        int windowHeight = Math.Min(FixedWindowHeight, Console.WindowHeight);

        int boardWidth = snapshot.Width + 2;
        int boardHeight = snapshot.Height + 2;
        int gap = 3;
        int sidebarWidth = windowWidth - boardWidth - gap;

        PlayerDto? localPlayer = FindPlayer(snapshot, localPlayerId);

        List<string> sidebarLines = BuildSidebarLines(snapshot, localPlayer);

        int totalLines = Math.Max(boardHeight, sidebarLines.Count);
        int linesToDraw = Math.Min(totalLines, windowHeight);

        for (int y = 0; y < linesToDraw; y++)
        {
            string boardPart = y < boardHeight
                ? BuildBoardLine(y, snapshot, localPlayerId)
                : new string(' ', boardWidth);

            string sidebarPart = y < sidebarLines.Count
                ? FitToWidth(sidebarLines[y], sidebarWidth)
                : "";

            string line = boardPart.PadRight(boardWidth) + new string(' ', gap) + sidebarPart;
            line = FitToWidth(line, windowWidth);

            Console.SetCursorPosition(0, y);
            Console.Write(line.PadRight(windowWidth));
        }

        ClearRemainingLines(linesToDraw, windowWidth, windowHeight);
    }

    private static string BuildBoardLine(int screenY, GameSnapshotDto snapshot, int localPlayerId)
    {
        var line = new StringBuilder();

        if (screenY == 0)
        {
            line.Append('┌');
            line.Append(new string('─', snapshot.Width));
            line.Append('┐');
            return line.ToString();
        }

        if (screenY == snapshot.Height + 1)
        {
            line.Append('└');
            line.Append(new string('─', snapshot.Width));
            line.Append('┘');
            return line.ToString();
        }

        int y = screenY - 1;
        line.Append('│');

        for (int x = 0; x < snapshot.Width; x++)
        {
            char symbol = FindSymbolAt(snapshot, x, y, localPlayerId);
            line.Append(symbol);
        }

        line.Append('│');
        return line.ToString();
    }

    private static char FindSymbolAt(GameSnapshotDto snapshot, int x, int y, int localPlayerId)
    {
        foreach (PlayerDto player in snapshot.Players)
        {
            if (player.X == x && player.Y == y)
            {
                if (player.Id == localPlayerId)
                    return '¶';

                return PlayerIdToSymbol(player.Id);
            }
        }

        CellDto? cell = FindCell(snapshot, x, y);

        if (cell == null)
            return '?';

        if (cell.HasEnemy)
            return cell.EnemySymbol;

        if (cell.Items.Count > 0)
            return cell.Items[0].Symbol;

        return cell.TileSymbol;
    }

    private static char PlayerIdToSymbol(int id)
    {
        if (id >= 1 && id <= 9)
            return id.ToString()[0];

        return 'P';
    }

    private static List<string> BuildSidebarLines(GameSnapshotDto snapshot, PlayerDto? player)
    {
        var lines = new List<string>
        {
            "RPG Multiplayer",
            $"Theme: {snapshot.ThemeName}",
            "",
            "Controls:",
            "W/A/S/D - move",
            "E - pick up item",
            "I - open inventory",
            "Esc - close inventory",
            "1/2/3 - attack",
            "Q - quit",
            ""
        };

        if (player == null)
        {
            lines.Add("Waiting for player data...");
            return lines;
        }

        if (!string.IsNullOrWhiteSpace(player.Message))
        {
            lines.Add($"Message: {player.Message}");
            lines.Add("");
        }

        lines.Add($"You are player: {player.Id}");
        lines.Add($"Name: {player.Name}");
        lines.Add($"Position: ({player.X},{player.Y})");
        lines.Add($"Health: {player.Health}");
        lines.Add($"Coins: {player.Coins}   Gold: {player.Gold}");
        lines.Add($"Strength: {player.Strength}");
        lines.Add($"Dexterity: {player.Dexterity}");
        lines.Add($"Luck: {player.Luck}");
        lines.Add($"Aggression: {player.Aggression}");
        lines.Add($"Wisdom: {player.Wisdom}");
        lines.Add($"Left hand: {player.LeftHand}");
        lines.Add($"Right hand: {player.RightHand}");
        lines.Add("");

        CellDto? currentCell = FindCell(snapshot, player.X, player.Y);

        if (currentCell != null && currentCell.HasEnemy)
        {
            lines.Add($"Enemy: {currentCell.EnemyName}");
            lines.Add($"Enemy health: {currentCell.EnemyHealth}");
            lines.Add($"Enemy attack: {currentCell.EnemyAttack}");
            lines.Add($"Enemy armor: {currentCell.EnemyArmor}");
            lines.Add("");
        }

        if (currentCell != null)
        {
            if (currentCell.Items.Count == 0)
            {
                lines.Add("No items on this tile.");
            }
            else
            {
                lines.Add("Items on this tile:");

                foreach (ItemDto item in currentCell.Items)
                    lines.Add($"- {item.Name}: {item.Description}");
            }
        }

        lines.Add("");

        if (player.IsInventoryOpen)
        {
            lines.Add("Inventory:");
            lines.Add("Up/Down - select");
            lines.Add("L/R - equip");
            lines.Add("X - drop");
            lines.Add("Z/C/U - unequip");
            lines.Add("");

            if (player.Inventory.Count == 0)
            {
                lines.Add("Inventory is empty.");
            }
            else
            {
                for (int i = 0; i < player.Inventory.Count; i++)
                {
                    string prefix = i == player.SelectedIndex ? "> " : "  ";
                    lines.Add($"{prefix}{i + 1}. {player.Inventory[i].Name}");
                }

                if (player.SelectedIndex >= 0 && player.SelectedIndex < player.Inventory.Count)
                    lines.Add($"Desc: {player.Inventory[player.SelectedIndex].Description}");
            }

            lines.Add("");
        }

        lines.Add("Players:");

        foreach (PlayerDto other in snapshot.Players)
        {
            lines.Add($"{other.Id}: {other.Name} ({other.X},{other.Y}) HP: {other.Health}");
        }

        lines.Add("");
        lines.Add("Recent log:");

        foreach (string entry in snapshot.RecentLogEntries)
            lines.Add(entry);

        return lines;
    }

    private static CellDto? FindCell(GameSnapshotDto snapshot, int x, int y)
    {
        foreach (CellDto cell in snapshot.Cells)
        {
            if (cell.X == x && cell.Y == y)
                return cell;
        }

        return null;
    }

    private static PlayerDto? FindPlayer(GameSnapshotDto snapshot, int id)
    {
        foreach (PlayerDto player in snapshot.Players)
        {
            if (player.Id == id)
                return player;
        }

        return null;
    }

    private static string FitToWidth(string text, int width)
    {
        if (string.IsNullOrEmpty(text) || width <= 0)
            return "";

        if (text.Length <= width)
            return text;

        return text.Substring(0, width);
    }

    private static void ClearRemainingLines(int fromLine, int width, int height)
    {
        for (int y = fromLine; y < height; y++)
        {
            Console.SetCursorPosition(0, y);
            Console.Write(new string(' ', width));
        }
    }
}