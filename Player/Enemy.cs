namespace Rpg;

public sealed class Enemy : Fighter
{
    public string Name {get;}
    public int Attack {get;}
    public int Armor {get;}

    public Enemy(string name, int health, int attack, int armor) : base(health){
        Name = name;
        Attack = attack;
        Armor = armor;
    }
}