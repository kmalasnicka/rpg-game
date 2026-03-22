namespace Rpg;

public abstract class Item : IDrawable, IInteractable{
    public abstract char Symbol { get; }
    public abstract string Name { get; }
    public virtual string Description => ""; 
    public virtual bool TryEquip(Player player, bool targetLeftHand) => false;
    public virtual void Interact(Player player) => player.Inventory.Add(this);
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

public abstract class Weapon : Item, IEquipable{
    public abstract int Damage { get; }
    public override string Description => $"Damage: {Damage}";
    public abstract void EquipLeft(Player player);
    public abstract void EquipRight(Player player);
}

public abstract class OneHandWeapon : Weapon{
    public override bool TryEquip(Player player, bool targetLeftHand){
        var left = player.Equipment.Left;
        var right = player.Equipment.Right;

        if(targetLeftHand){ //chcemy wlozyc do lewej reki
            if(left != null && ReferenceEquals(left, right)){ //bron dwureczna
                player.Inventory.Add(left); 
                player.Equipment.Clear(); //czyscimy obie rece
            }else{ 
                if(left != null) player.Inventory.Add(left);
                player.Equipment.SetLeft(null); 
            }
            player.Equipment.SetLeft(this);//ustawiamy bron w rece
        }else{//chcemy wlozyc do prawej reki
            if(right != null && ReferenceEquals(left, right)){
                player.Inventory.Add(right);
                player.Equipment.Clear();
            }else{
                if(right != null) player.Inventory.Add(right);
                player.Equipment.SetRight(null);
            }
            player.Equipment.SetRight(this);
        }
        return true;
    }

    public override void EquipLeft(Player player) => player.Equipment.SetLeft(this);
    public override void EquipRight(Player player) => player.Equipment.SetRight(this);
}

public abstract class TwoHandWeapon : Weapon{
    public override bool TryEquip(Player player, bool targetLeftHand){
        var left = player.Equipment.Left;
        var right = player.Equipment.Right;
        if(left != null) player.Inventory.Add(left);
        if(right != null && !ReferenceEquals(left, right)) player.Inventory.Add(right); //jesli cos innego niz w lewej to dodajemy do inventory
        player.Equipment.Clear();
        player.Equipment.SetLeft(this);
        player.Equipment.SetRight(this);
        return true;
    }

    public override void EquipLeft(Player player){
        player.Equipment.Clear();
        player.Equipment.SetLeft(this);
        player.Equipment.SetRight(this);
    }

    public override void EquipRight(Player player){
        player.Equipment.Clear();
        player.Equipment.SetLeft(this);
        player.Equipment.SetRight(this);
    }
}

public sealed class Dagger : OneHandWeapon{
    public override int Damage => 2;
    public override char Symbol => 'd';
    public override string Name => "Dagger";
}

public sealed class Sword : OneHandWeapon{
    public override int Damage => 5;
    public override char Symbol => 's';
    public override string Name => "Sword";
}

public sealed class Axe : TwoHandWeapon{
    public override int Damage => 10;
    public override char Symbol => 'a';
    public override string Name => "Axe";
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