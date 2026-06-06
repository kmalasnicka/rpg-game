namespace Rpg;

public sealed class ConsoleInputController
{
    public PlayerActionDto? ReadAction(ConsoleKey key, GameSnapshotDto snapshot, int localPlayerId)
    {
        PlayerDto? player = FindPlayer(snapshot, localPlayerId);

        if (player == null)
            return null;

        if (key == ConsoleKey.Q)
            return Create(localPlayerId, "Quit");

        if (key == ConsoleKey.W)
            return CreateMove(localPlayerId, 0, -1);

        if (key == ConsoleKey.S)
            return CreateMove(localPlayerId, 0, 1);

        if (key == ConsoleKey.A)
            return CreateMove(localPlayerId, -1, 0);

        if (key == ConsoleKey.D)
            return CreateMove(localPlayerId, 1, 0);

        if (key == ConsoleKey.E)
            return Create(localPlayerId, "Pickup");

        if (key == ConsoleKey.I)
            return Create(localPlayerId, "OpenInventory");

        if (key == ConsoleKey.Escape)
            return Create(localPlayerId, "CloseInventory");

        if (key == ConsoleKey.UpArrow)
            return Create(localPlayerId, "SelectUp");

        if (key == ConsoleKey.DownArrow)
            return Create(localPlayerId, "SelectDown");

        if (key == ConsoleKey.L)
            return new PlayerActionDto
            {
                PlayerId = localPlayerId,
                ActionName = "Equip",
                LeftHand = true
            };

        if (key == ConsoleKey.R)
            return new PlayerActionDto
            {
                PlayerId = localPlayerId,
                ActionName = "Equip",
                LeftHand = false
            };

        if (key == ConsoleKey.X)
            return Create(localPlayerId, "Drop");

        if (key == ConsoleKey.Z)
            return Create(localPlayerId, "UnequipLeft");

        if (key == ConsoleKey.C)
            return Create(localPlayerId, "UnequipRight");

        if (key == ConsoleKey.U)
            return Create(localPlayerId, "UnequipBoth");

        if (key == ConsoleKey.D1)
            return CreateAttack(localPlayerId, "Normal");

        if (key == ConsoleKey.D2)
            return CreateAttack(localPlayerId, "Stealth");

        if (key == ConsoleKey.D3)
            return CreateAttack(localPlayerId, "Magical");

        return null;
    }

    private static PlayerDto? FindPlayer(GameSnapshotDto snapshot, int playerId)
    {
        foreach (PlayerDto player in snapshot.Players)
        {
            if (player.Id == playerId)
                return player;
        }

        return null;
    }

    private static PlayerActionDto Create(int playerId, string actionName)
    {
        return new PlayerActionDto
        {
            PlayerId = playerId,
            ActionName = actionName
        };
    }

    private static PlayerActionDto CreateMove(int playerId, int dx, int dy)
    {
        return new PlayerActionDto
        {
            PlayerId = playerId,
            ActionName = "Move",
            Dx = dx,
            Dy = dy
        };
    }

    private static PlayerActionDto CreateAttack(int playerId, string styleName)
    {
        return new PlayerActionDto
        {
            PlayerId = playerId,
            ActionName = "Attack",
            AttackStyleName = styleName
        };
    }
}