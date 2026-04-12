using System.Text;

namespace Rpg;

public sealed class Renderer{
    private const int FixedWindowWidth = 120;
    private const int FixedWindowHeight = 40;

    public void Initialize(){
        Console.Clear();
        Console.CursorVisible = false;
    }

    public void Shutdown(){ //kursor znowu widoczny
        Console.CursorVisible = true;
    }

    public void Render(Room room, Player player, List<GameAction> actions, int selectedIndex, string message, IReadOnlyList<string> instructions, bool inventoryOpen) {
        int windowWidth = Math.Min(FixedWindowWidth, Console.WindowWidth);
        int windowHeight = Math.Min(FixedWindowHeight, Console.WindowHeight);
//rozmiar planszy i sidebara (+2 na ramke)
        int boardWidth = room.Width + 2;
        int boardHeight = room.Height + 2;
        int gap = 3;
        int sidebarWidth = windowWidth - boardWidth - gap;

        var playerCell = room.GetCell(player.Position);

        var sidebarLines = BuildSidebarLines(player, inventoryOpen, selectedIndex, playerCell, message, instructions);
        int totalLines = Math.Max(boardHeight, sidebarLines.Count);
        int linesToDraw = Math.Min(totalLines, windowHeight);

        for (int y = 0; y < linesToDraw; y++){
            string boardPart = y < boardHeight ? BuildBoardLine(y, room, player) : new string(' ', boardWidth);
            string sidebarPart = y < sidebarLines.Count ? FitToWidth(sidebarLines[y], sidebarWidth) : "";
            string line = boardPart.PadRight(boardWidth) + new string(' ', gap) + sidebarPart;
            line = FitToWidth(line, windowWidth);
            Console.SetCursorPosition(0, y);
            Console.Write(line.PadRight(windowWidth));
        }
        ClearRemainingLines(linesToDraw, windowWidth, windowHeight);
    }

    private void ClearRemainingLines(int fromLine, int width, int height){
        for (int y = fromLine; y < height; y++){
            Console.SetCursorPosition(0, y);
            Console.Write(new string(' ', width));
        }
    }

    private string FitToWidth(string text, int width){
        if (string.IsNullOrEmpty(text) || width <= 0) return "";
        if (text.Length <= width) return text;
        return text.Substring(0, width);
    }

    private string BuildBoardLine(int screenY, Room room, Player player){
        var line = new StringBuilder();
        if (screenY == 0){ //gorna ramka
            line.Append('┌');
            line.Append(new string('─', room.Width));
            line.Append('┐');
            return line.ToString();
        }

        if (screenY == room.Height + 1){
            line.Append('└'); //dolna ramka
            line.Append(new string('─', room.Width));
            line.Append('┘');
            return line.ToString();
        }

        int y = screenY - 1;
        line.Append('│'); //lewa ramka

        for (int x = 0; x < room.Width; x++){
            var pos = new Position(x, y);
            var cell = room.GetCell(pos); 

            if (pos.X == player.Position.X && pos.Y == player.Position.Y){
                line.Append('¶');
            } else if (cell.HasEnemy()) {
                line.Append('E');
            } else if (cell.ItemsOnCell.Count > 0) {
                line.Append(cell.ItemsOnCell[0].Symbol);
            } else {
                line.Append(cell.Tile.Symbol);
            }
        }

        line.Append('│'); //prawa ramka
        return line.ToString();
    }

    private List<string> BuildSidebarLines(Player player, bool inventoryOpen, int selectedIndex, Cell playerCell, string message, IReadOnlyList<string> instructions)
    {
        var lines = new List<string>{
            "RPG Game",
            "",
            "Instructions:"
        };

        foreach (var instruction in instructions)
            lines.Add(instruction);

        lines.Add("");

        if (!string.IsNullOrWhiteSpace(message)){
            lines.Add($"Message: {message}");
            lines.Add("");
        }

        lines.Add($"Position: {player.Position}");
        lines.Add($"Health: {player.Health}");
        lines.Add($"Coins: {player.Wallet.Coins}   Gold: {player.Wallet.Gold}");
        lines.Add($"Strength: {player.EffectiveStrength}");
        lines.Add($"Dexterity: {player.EffectiveDexterity}");
        lines.Add($"Luck: {player.EffectiveLuck}");
        lines.Add($"Aggression: {player.EffectiveAggression}");
        lines.Add($"Wisdom: {player.EffectiveWisdom}");
        lines.Add($"Left hand: {(player.Equipment.Left?.Name ?? "-")}");
        lines.Add($"Right hand: {(player.Equipment.Right?.Name ?? "-")}");
        lines.Add("");

        if (playerCell.HasEnemy() && playerCell.Enemy != null)
        {
            lines.Add($"Enemy: {playerCell.Enemy.Name}");
            lines.Add($"Enemy Health: {playerCell.Enemy.Health}");
            lines.Add($"Enemy Attack: {playerCell.Enemy.Attack}");
            lines.Add($"Enemy Armor: {playerCell.Enemy.Armor}");
            lines.Add("");
        }

        if (playerCell.ItemsOnCell.Count == 0){
            lines.Add("No items on this tile");
        } else{
            lines.Add("Items on this tile:");
            foreach (var item in playerCell.ItemsOnCell)
                lines.Add($"- {item.Name}: {item.Description}");
        }
        lines.Add("");

        if (inventoryOpen){
            lines.Add("Inventory:");
            lines.Add($"Items: {player.Inventory.Count}");

            for (int i = 0; i < player.Inventory.Count; i++){
                string prefix = i == selectedIndex ? "> " : "  ";
                lines.Add($"{prefix}{i + 1}. {player.Inventory.Items[i].Name}");
            }

            if (player.Inventory.Count > 0 && selectedIndex >= 0 && selectedIndex < player.Inventory.Count){
                lines.Add($"Desc: {player.Inventory.Items[selectedIndex].Description}");
            }
        }
        return lines;
    }

    public void RenderGameOver(string message)
    {
        Console.Clear();
        Console.SetCursorPosition(0, 0);
        Console.WriteLine("GAME OVER");
        Console.WriteLine();
        Console.WriteLine(message);
        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
    }
}