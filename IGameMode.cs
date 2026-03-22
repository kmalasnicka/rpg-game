namespace Rpg;

public interface IGameMode{
    List<GameAction> GetActions(Game game); //kazdy tryb zwraca liste akcji dostepnych 
}