namespace Rpg;

public sealed class NetworkMessageDto
{
    public string Type { get; set; } = "";

    public LoginDto? Login { get; set; }
    public PlayerActionDto? Action { get; set; }
    public GameUpdateDto? Update { get; set; }

    public string ErrorMessage { get; set; } = "";
}

public sealed class LoginDto
{
    public string PlayerName { get; set; } = "Player";
}

public sealed class PlayerActionDto
{
    public int PlayerId { get; set; }

    public string ActionName { get; set; } = "";

    public int Dx { get; set; }
    public int Dy { get; set; }

    public bool LeftHand { get; set; }

    public string AttackStyleName { get; set; } = "";
}

public sealed class GameUpdateDto
{
    public int LocalPlayerId { get; set; }
    public GameSnapshotDto Snapshot { get; set; } = new();
}