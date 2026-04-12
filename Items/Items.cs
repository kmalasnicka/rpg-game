namespace Rpg;

public abstract class Item : IDrawable, IInteractable{
    public abstract char Symbol { get; }
    public abstract string Name { get; }
    public virtual string Description => ""; 
    public virtual bool TryEquip(Player player, bool targetLeftHand) => false;
    public virtual void Interact(Player player) => player.Inventory.Add(this);
    
    public virtual CombatValues GetCombatValues(Player player, IAttackStyle style) => style.UseWithoutWeapon(player);
    public virtual int GetDamageModifier() => 0;
    public virtual int GetStrengthModifier() => 0;
    public virtual int GetDexterityModifier() => 0;
    public virtual int GetAggressionModifier() => 0;
    public virtual int GetWisdomModifier() => 0;
    public virtual int GetLuckModifier() => 0;
}

public abstract class Currency : Item{
    public abstract int Amount { get; }
    public sealed override void Interact(Player player)
    {
        AddToWallet(player, Amount);
    }
    protected abstract void AddToWallet(Player player, int amount); 
}

public sealed class Coin : Currency{
    public override int Amount => 1;
    public override char Symbol => 'c';
    public override string Name => "Coin";
    public override string Description => $"Coin worth: {Amount}";
    protected override void AddToWallet(Player player, int amount) => player.Wallet.AddCoins(amount);
}

public sealed class Gold : Currency{
    public override int Amount => 1;
    public override char Symbol => 'g';
    public override string Name => "Gold";
    public override string Description => $"Gold worth: {Amount}";
    protected override void AddToWallet(Player player, int amount) => player.Wallet.AddGold(amount);
}

public sealed class Bottle : Item{
    public override char Symbol => 'b';
    public override string Name => "Bottle";
    public override string Description => "An empty bottle";
}

public sealed class Feather : Item{
    public override char Symbol => 'f';
    public override string Name => "Feather";
    public override string Description => "A light feather";
}

public sealed class ScrapMetal : Item{
    public override char Symbol => 'm';
    public override string Name => "Scrap Metal";
    public override string Description => "A useless piece of metal";
}

public sealed class EmptyHandsItem : Item{
    public override char Symbol => ' ';
    public override string Name => "Empty hands";
    public override string Description => "No weapon equipped.";
}