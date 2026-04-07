namespace Rpg;

public sealed class WorldMode : IGameMode
{
    public List<GameAction> GetActions(Game game)
    {
        var actions = new List<GameAction>
        {
            new GameAction(ConsoleKey.W, "Move up", g => true, g => g.TryMove(0, -1)),
            new GameAction(ConsoleKey.S, "Move down", g => true, g => g.TryMove(0, 1)),
            new GameAction(ConsoleKey.A, "Move left", g => true, g => g.TryMove(-1, 0)),
            new GameAction(ConsoleKey.D, "Move right", g => true, g => g.TryMove(1, 0)),
            new GameAction(ConsoleKey.Q, "Quit game", g => true, g => g.Quit())
        };

        if (game.SupportsLoot()){
            if(game.HasItemOnGround()) actions.Add(new GameAction(ConsoleKey.E, "Pick up item", g => g.HasItemOnGround(), g => g.TryPickup(), "There is nothing here."));
            if(game.HasInventory() || game.HasAnyEquipped()) actions.Add(new GameAction(ConsoleKey.I, "Open inventory", g => true, g => g.OpenInventory()));
        }
        return actions;
    }
}

public sealed class InventoryMode : IGameMode
{
    public List<GameAction> GetActions(Game game)
    {
        var actions = new List<GameAction>
        {
            new GameAction(ConsoleKey.UpArrow, "Select previous item", g => g.HasInventory(), g => g.SelectUp(), "Inventory is empty."),
            new GameAction(ConsoleKey.DownArrow, "Select next item", g => g.HasInventory(), g => g.SelectDown(), "Inventory is empty."),
            new GameAction(ConsoleKey.X, "Drop selected item", g => g.HasInventory(), g => g.DropSelected(), "Inventory is empty."),
            new GameAction(ConsoleKey.Escape, "Close inventory", g => true, g => g.CloseInventory())
        };

        if (game.SupportsWeapons())
        {
            actions.Add(new GameAction(ConsoleKey.L, "Equip selected in left hand", g => g.HasInventory(), g => g.EquipSelected(left: true), "Inventory is empty."));
            actions.Add(new GameAction(ConsoleKey.R, "Equip selected in right hand", g => g.HasInventory(), g => g.EquipSelected(left: false), "Inventory is empty."));
            actions.Add(new GameAction(ConsoleKey.Z, "Unequip left hand", g => g.HasLeftEquipped(), g => g.UnequipLeft(), "Left hand is empty."));
            actions.Add(new GameAction(ConsoleKey.C, "Unequip right hand", g => g.HasRightEquipped(), g => g.UnequipRight(), "Right hand is empty."));
            actions.Add(new GameAction(ConsoleKey.U, "Unequip both hands", g => g.HasAnythingToUnequip(), g => g.UnequipBoth(), "You have nothing equipped."));
        }

        return actions;
    }
}