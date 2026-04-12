namespace Rpg;

public abstract class ItemModifier : Item{
    protected Item InnerItem { get; }
    protected ItemModifier(Item innerItem){
        InnerItem = innerItem;
    }

    public override string Name => InnerItem.Name;
    public override string Description => InnerItem.Description;
    public override char Symbol => InnerItem.Symbol;

    public override bool TryEquip(Player player, bool targetLeftHand) => InnerItem.TryEquip(player, targetLeftHand);
    public override void Interact(Player player){
        player.Inventory.Add(this);
    }

    public override CombatValues GetCombatValues(Player player, IAttackStyle style) => InnerItem.GetCombatValues(player, style);
    public override int GetLuckModifier() => InnerItem.GetLuckModifier();
    public override int GetDamageModifier() => InnerItem.GetDamageModifier();
    public override int GetStrengthModifier() => InnerItem.GetStrengthModifier();
    public override int GetDexterityModifier() => InnerItem.GetDexterityModifier();
    public override int GetAggressionModifier() => InnerItem.GetAggressionModifier();
    public override int GetWisdomModifier() => InnerItem.GetWisdomModifier();
}

public sealed class UnluckyItemModifier : ItemModifier{
    public UnluckyItemModifier(Item innerItem) : base(innerItem) { }

    public override string Name => $"{InnerItem.Name} (Unlucky)";
    public override int GetLuckModifier() => InnerItem.GetLuckModifier() - 5;
}