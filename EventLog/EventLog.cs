namespace Rpg;

public sealed class EventLog{
    private static readonly EventLog _instance = new EventLog(); //jedyny obiekt EventLog
    private IEventLog? _current; 
    private EventLog() {} //blokuje tworzenie nowych obiektow z zewnatrz
    public static EventLog Instance => _instance; //dostep do tej jednej instancji
    
    public static IEventLog Current { //aktualna implementacja logera 
        get{
            if(_instance._current == null) throw new InvalidOperationException("Event log is not configured.");
            return _instance._current;
        }
    }

    public void Configure(IEventLog log){
        _current = log;
    }
}