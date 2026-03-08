using System.Collections.Generic; 
namespace Rpg;

public class Player{
    public Position Position { get; private set; }
    public Attributes Attributes { get; }
    public Inventory Inventory { get; } //itemy ktore gracz nosi
    public Equipment Equipment {get;} //przedmioty trzymane w rekach
    public Wallet Wallet{ get;}

    public Player(Position startPosition, Attributes attributes){
        Position = startPosition;
        Attributes = attributes;
        Inventory = new Inventory();
        Equipment = new Equipment();
        Wallet = new Wallet();
    }
    public void SetPosition(Position position){
        Position = position;
    }
}

public class Attributes{
    public int Strength {get; private set; }
    public int Dexterity {get; private set;}
    public int Health {get; private set;}
    public int Luck {get; private set;}
    public int Aggression {get; private set;}
    public int Wisdom {get; private set;}

    public Attributes(int strength, int dexterity, int health, int luck, int aggression, int wisdom){
        Strength = strength;
        Dexterity = dexterity;
        Health = health;
        Luck = luck;
        Aggression = aggression;
        Wisdom = wisdom;
    }
}
public class Equipment{
    public Item? Left {get; private set;}
    public Item? Right {get; private set;}

    public void Clear(){
        Left = null;
        Right = null;
    }

    public void SetLeft(Item? item) => Left = item;
    public void SetRight(Item? item) => Right = item;

    public Item? RemoveLeft(){
        var item = Left; 
        if(item == null) return null; 
        if(ReferenceEquals(Left, Right)){ //sprawdzamy czy jest dwureczna bron 
            Left = null;
            Right = null;
            return item;
        }
        Left = null;
        return item;
    }

    public Item? RemoveRight()
    {
        var item = Right;
        if (item == null) return null;
        if (ReferenceEquals(Left, Right)){
            Left = null;
            Right = null;
            return item;
        }
        Right = null;
        return item;
    }
}

public class Inventory{
    private readonly List<Item> _items = new(); 
    public IReadOnlyList<Item> Items => _items; 
    public int Count => _items.Count;
    public void Add(Item item) => _items.Add(item);
    public Item RemoveAt(int index) {
        var item = _items[index];
        _items.RemoveAt(index);
        return item;
    }
}

public class Wallet{
    public int Coins { get; private set; }
    public int Gold { get; private set; }
    public void AddCoins(int coins) => Coins += coins;
    public void AddGold(int gold) => Gold += gold;
}