namespace Rpg;

public sealed class InstructionBuilder{
    public IReadOnlyList<string> Build(Game game, IReadOnlyList<GameAction> actions){ //buduje liste tekstow do wyswietlenia
        var instructions = new List<string>(); //pusta lista instrukcji
        AddActionInstructions(instructions, actions); //dodajemy podstawowe instrukcje 
        AddWorldInstructions(instructions, game);
        AddInventoryInstructions(instructions, game);
        return instructions;
    }

    private void AddActionInstructions(List<string> instructions, IReadOnlyList<GameAction> actions){
        foreach (var action in actions) instructions.Add($"{FormatKey(action.Key)} - {action.Description}");
    }

    private void AddWorldInstructions(List<string> instructions, Game game){
        if (game.IsInventoryOpen) return; 
        if (game.HasItemOnGround()) instructions.Add("You are standing on an item. Press E to pick it up.");
        else if (game.HasItemsAnywhere()) instructions.Add("Explore the dungeon to find items.");
        else instructions.Add("There are no items on the ground.");
    }

    private void AddInventoryInstructions(List<string> instructions, Game game){
        if (!game.IsInventoryOpen){
            if (!game.HasInventory()) instructions.Add("Inventory is empty.");
            if (game.HasAnyEquipped()) instructions.Add("Open inventory to manage equipped items.");
            return;
        }

        if (game.HasInventory()){
            instructions.Add("Use Up/Down to select an item.");
            instructions.Add("Press L/R to equip, X to drop, Esc to close.");
        } else {
            instructions.Add("Inventory is empty. Press Esc to close.");
        }

        if (game.HasLeftEquipped() || game.HasRightEquipped()) instructions.Add("Press Z/C/U to unequip items.");
    }

    private string FormatKey(ConsoleKey key){
        return key switch {
            ConsoleKey.UpArrow => "UpArrow",
            ConsoleKey.DownArrow => "DownArrow",
            ConsoleKey.LeftArrow => "LeftArrow",
            ConsoleKey.RightArrow => "RightArrow",
            ConsoleKey.Escape => "Esc",
            _ => key.ToString()
        };
    }
}