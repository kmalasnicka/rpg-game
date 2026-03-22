namespace Rpg;

public sealed class GameAction{ //zamiast switch akcja to obiekt z klawiszem, opisem, warunkiem wykonania, metoda wykonania i komunikatem bledu
    public ConsoleKey Key { get; } 
    public string Description { get; }
    public Func<Game, bool> CanExecute { get; } //sprawdza czy akcja jest teraz dozwolona
    public Action<Game> Execute { get; } //kod wykonywany po nacisnieciu klawisza
    public string CannotExecuteMessage { get; }

    public GameAction(ConsoleKey key, string description, Func<Game, bool> canExecute, Action<Game> execute, string cannotExecuteMessage = "Cannot perform action"){
        Key = key;
        Description = description;
        CanExecute = canExecute;
        Execute = execute;
        CannotExecuteMessage = cannotExecuteMessage;
    }
}