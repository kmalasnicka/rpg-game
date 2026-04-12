namespace Rpg;

public class Wallet{
    public int Coins { get; private set; }
    public int Gold { get; private set; }
    public void AddCoins(int coins) => Coins += coins;
    public void AddGold(int gold) => Gold += gold;
}