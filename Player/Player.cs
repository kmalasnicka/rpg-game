using System;
using System.Collections.Generic;

namespace Rpg;

public class Player : Fighter{
    public Position Position { get; private set; }
    public Attributes Attributes { get; }
    public Inventory Inventory { get; }
    public Equipment Equipment { get; }
    public Wallet Wallet { get; }

    public Player(Position startPosition, Attributes attributes, int health) : base(health){
        Position = startPosition;
        Attributes = attributes;
        Inventory = new Inventory();
        Equipment = new Equipment();
        Wallet = new Wallet();
    }

    public void SetPosition(Position position){
        Position = position;
    }

    public int EffectiveStrength =>
        Attributes.Strength
        + SumEquipmentModifiers(item => item.GetStrengthModifier());

    public int EffectiveDexterity =>
        Attributes.Dexterity
        + SumEquipmentModifiers(item => item.GetDexterityModifier());

    public int EffectiveLuck =>
        Attributes.Luck
        + SumEquipmentModifiers(item => item.GetLuckModifier())
        + SumInventoryItemModifiers(item => item.GetLuckModifier());

    public int EffectiveAggression =>
        Attributes.Aggression
        + SumEquipmentModifiers(item => item.GetAggressionModifier());

    public int EffectiveWisdom =>
        Attributes.Wisdom
        + SumEquipmentModifiers(item => item.GetWisdomModifier());

    private int SumEquipmentModifiers(Func<Item, int> selector){
        int total = 0;

        if (Equipment.Left != null)
            total += selector(Equipment.Left);

        if (Equipment.Right != null && !ReferenceEquals(Equipment.Left, Equipment.Right))
            total += selector(Equipment.Right);

        return total;
    }

    private int SumInventoryItemModifiers(Func<Item, int> selector){
        int total = 0;

        foreach (var item in Inventory.Items){
            if (item is Weapon)
                continue;

            total += selector(item);
        }

        return total;
    }
}