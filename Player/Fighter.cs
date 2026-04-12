namespace Rpg;

public abstract class Fighter
{
    public int Health {get; protected set;}
    public bool IsDead => Health <= 0;

    protected Fighter(int health){
        Health = health;
    }

    public virtual void TakeDamage(int amount){
        Health = Math.Max(0, Health - amount);
    }
}