namespace Rpg;

public sealed class PlayerState
{
    public Player Player { get; }

    public bool IsInventoryOpen { get; set; }
    public int SelectedIndex { get; set; }
    public string Message { get; set; } = "";
    public bool IsGameOver { get; set; }

    public PlayerState(Player player)
    {
        Player = player;
    }
}