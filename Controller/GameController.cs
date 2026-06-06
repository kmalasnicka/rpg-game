namespace Rpg;

public sealed class GameController
{
    private readonly GameModel _model;
    private readonly Dictionary<string, Action<PlayerActionDto>> _handlers;
    private readonly Dictionary<string, Func<IAttackStyle>> _attackStyles;

    public GameController(GameModel model)
    {
        _model = model;

        _handlers = new Dictionary<string, Action<PlayerActionDto>>(StringComparer.OrdinalIgnoreCase)
        {
            ["Move"] = HandleMove,
            ["Pickup"] = HandlePickup,
            ["OpenInventory"] = HandleOpenInventory,
            ["CloseInventory"] = HandleCloseInventory,
            ["SelectUp"] = HandleSelectUp,
            ["SelectDown"] = HandleSelectDown,
            ["Equip"] = HandleEquip,
            ["UnequipLeft"] = HandleUnequipLeft,
            ["UnequipRight"] = HandleUnequipRight,
            ["UnequipBoth"] = HandleUnequipBoth,
            ["Drop"] = HandleDrop,
            ["Attack"] = HandleAttack
        };

        _attackStyles = new Dictionary<string, Func<IAttackStyle>>(StringComparer.OrdinalIgnoreCase)
        {
            ["Normal"] = () => new NormalAttack(),
            ["Stealth"] = () => new StealthAttack(),
            ["Magical"] = () => new MagicalAttack()
        };
    }

    public void HandleAction(PlayerActionDto action)
    {
        if (!_handlers.TryGetValue(action.ActionName, out Action<PlayerActionDto>? handler))
        {
            _model.SetMessage(action.PlayerId, $"Unknown action: {action.ActionName}");
            return;
        }

        handler(action);
    }

    private void HandleMove(PlayerActionDto action)
    {
        _model.MovePlayer(action.PlayerId, action.Dx, action.Dy);
    }

    private void HandlePickup(PlayerActionDto action)
    {
        _model.PickUpItem(action.PlayerId);
    }

    private void HandleOpenInventory(PlayerActionDto action)
    {
        _model.OpenInventory(action.PlayerId);
    }

    private void HandleCloseInventory(PlayerActionDto action)
    {
        _model.CloseInventory(action.PlayerId);
    }

    private void HandleSelectUp(PlayerActionDto action)
    {
        _model.SelectUp(action.PlayerId);
    }

    private void HandleSelectDown(PlayerActionDto action)
    {
        _model.SelectDown(action.PlayerId);
    }

    private void HandleEquip(PlayerActionDto action)
    {
        _model.EquipSelected(action.PlayerId, action.LeftHand);
    }

    private void HandleUnequipLeft(PlayerActionDto action)
    {
        _model.UnequipLeft(action.PlayerId);
    }

    private void HandleUnequipRight(PlayerActionDto action)
    {
        _model.UnequipRight(action.PlayerId);
    }

    private void HandleUnequipBoth(PlayerActionDto action)
    {
        _model.UnequipBoth(action.PlayerId);
    }

    private void HandleDrop(PlayerActionDto action)
    {
        _model.DropSelected(action.PlayerId);
    }

    private void HandleAttack(PlayerActionDto action)
    {
        if (!_attackStyles.TryGetValue(action.AttackStyleName, out Func<IAttackStyle>? createStyle))
        {
            _model.SetMessage(action.PlayerId, $"Unknown attack style: {action.AttackStyleName}");
            return;
        }

        _model.AttackEnemy(action.PlayerId, createStyle());
    }
}