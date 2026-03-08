using System.Text;

namespace Rpg;

public sealed class Renderer
{
    public void Initialize()
    {
        Console.Clear(); //czyscimy ekran
        Console.CursorVisible = false; //chowamy migajacy kursor
    }

    public void Shutdown()
    {
        Console.CursorVisible = true; 
    }

    public void Render(Room room, Player player, bool inventoryOpen, int selectedIndex, string message)
    {
        int boardHeight = room.Height;
        int boardWidth = room.Width;

        int totalHeight = boardHeight + 2; // +2 bo gorna i dolna ramka

        var playerCell = room.GetCell(player.Position); 

        for (int screenY = 0; screenY < totalHeight; screenY++)
        {
            string boardPart = BuildBoardLine(screenY, room, player, boardWidth, boardHeight);
            string sidebarPart = GetSidebarContent(screenY - 1, player, inventoryOpen, selectedIndex, playerCell, message);

            string fullLine = boardPart + "   " + sidebarPart; //plansza i sidebar razem

            Console.SetCursorPosition(0, screenY);
            Console.Write(fullLine.PadRight(Console.WindowWidth)); //wydluzamy linie do pelnej szerokosci okna, dodajemy spacje na koncu
        }
    }

    private string BuildBoardLine(int screenY, Room room, Player player, int boardWidth, int boardHeight)
    {
        var line = new StringBuilder();
        if (screenY == 0) //gorna ramka
        {
            line.Append('┌');
            line.Append(new string('─', boardWidth));
            line.Append('┐');
            return line.ToString();
        }

        if (screenY == boardHeight + 1) //dolna ramka
        {
            line.Append('└');
            line.Append(new string('─', boardWidth));
            line.Append('┘');
            return line.ToString();
        }
        int y = screenY - 1; 
        line.Append('│'); //lewa krawedz ramki
        for (int x = 0; x < boardWidth; x++){
            var pos = new Position(x, y);
            if (pos.X == player.Position.X && pos.Y == player.Position.Y){
                line.Append('¶');
            }else{
                var cell = room.GetCell(pos);
                if (cell.ItemsOnCell.Count > 0)
                    line.Append(cell.ItemsOnCell[0].Symbol); 
                else
                    line.Append(cell.Tile.Symbol); 
            }
        }
        line.Append('│'); //prawa krawedz ramki
        return line.ToString();
    }

    private string GetSidebarContent(int line, Player player, bool inventoryOpen, int selectedIndex, Cell playerCell, string message)
    {
        if (line == 0) return "RPG Game";
        if (line == 1) return "W/S/A/D move  E pick  I inventory  Q quit";
        if (line == 2 && !string.IsNullOrWhiteSpace(message)) return $"Message: {message}";
        if (line == 3) return $"Position: {player.Position}";
        if (line == 4) return $"Coins: {player.Wallet.Coins}   Gold: {player.Wallet.Gold}";
        if (line == 5) return $"Strength: {player.Attributes.Strength}   Dexterity: {player.Attributes.Dexterity}";
        if (line == 6) return $"Health: {player.Attributes.Health}   Luck: {player.Attributes.Luck}";
        if (line == 7) return $"Aggression: {player.Attributes.Aggression}   Wisdom: {player.Attributes.Wisdom}";
        if (line == 8) return $"Left hand: {(player.Equipment.Left?.Name ?? "-")}";
        if (line == 9) return $"Right hand: {(player.Equipment.Right?.Name ?? "-")}";
        if (line == 10){
            if (playerCell.ItemsOnCell.Count == 0)
                return "No items on this tile";
            return $"Number of items here: {playerCell.ItemsOnCell.Count}";
        }
        if (line == 11){
            if (playerCell.ItemsOnCell.Count > 0)
                return $"First item: {playerCell.ItemsOnCell[0].Name}";
        }
        if (line == 12){
            if (playerCell.ItemsOnCell.Count > 0)
                return $"First item info: {playerCell.ItemsOnCell[0].Description}";
        }
        if (line == 13){
            if (inventoryOpen)
                return "Inventory:";
            else
                return "Press I to open inventory";
        }
        if (line == 14 && inventoryOpen) return "↑/↓ select  L/R equip  X drop  Esc close";
        if (line == 15 && inventoryOpen) return "Z/C unequip left/right  U unequip both";
        if (line == 16 && inventoryOpen) return $"Items: {player.Inventory.Count}";
        if (inventoryOpen && line >= 17 && line <= 19){ 
            if (player.Inventory.Count == 0){
                if (line == 17) return "Inventory is empty";
                return ""; 
            }
            int visibleRows = 3;
            int start = selectedIndex - 1; 
            if (start < 0) start = 0; 
            if (start + visibleRows > player.Inventory.Count) 
                start = Math.Max(0, player.Inventory.Count - visibleRows);
            int itemIndex = start + (line - 17); //obliczamy do ktorej lini nalezy item 
            if (itemIndex < player.Inventory.Count){
                var item = player.Inventory.Items[itemIndex];
                if (itemIndex == selectedIndex)
                    return $"> {itemIndex + 1}. {item.Name}";
                return $"  {itemIndex + 1}. {item.Name}";
            }
        }
        if (line == 20 && inventoryOpen && player.Inventory.Count > 0){
            var item = player.Inventory.Items[selectedIndex]; //item ktory ma strzalke >
            return $"Desc: {item.Description}";
        }
        return "";
    }
}