namespace Rpg;

public class Attributes{
    public int Strength {get; private set; }
    public int Dexterity {get; private set;}
    public int Luck {get; private set;}
    public int Aggression {get; private set;}
    public int Wisdom {get; private set;}

    public Attributes(int strength, int dexterity, int luck, int aggression, int wisdom){
        Strength = strength;
        Dexterity = dexterity;
        Luck = luck;
        Aggression = aggression;
        Wisdom = wisdom;
    }
}