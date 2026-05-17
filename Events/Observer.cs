namespace Rpg;

public interface IObserver<T>{ //interface obserwatora
    void Notify(T value);
}

public interface ISubject<T>{ //interface obiektu powiadamiajacego 
    void Attach(IObserver<T> observer); //dodaje obserwatora
    void Detach(IObserver<T> observer); //usuwa obserwatora
    void NotifyAll(T value); //powiadamia wszystkich
}

public sealed class Subject<T> : ISubject<T>{
    private readonly List<IObserver<T>> _observers = new(); //lista obserwatorow 

    public void Attach(IObserver<T> observer){
        if (!_observers.Contains(observer)) //dodajemy tylko jak nie ma
            _observers.Add(observer);
    }

    public void Detach(IObserver<T> observer){ 
        _observers.Remove(observer);
    }

    public void NotifyAll(T value){
        foreach (var observer in _observers.ToList())
            observer.Notify(value);
    }
}