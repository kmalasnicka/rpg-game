namespace Rpg;

public sealed class FileEventLog : IEventLog{
    private readonly Queue<string> _recentEntries = new(); //kolejka ostatnich logow
    private readonly int _recentLimit; //ile logow trzymamy w kolejce
    public string FilePath { get; }

    public FileEventLog(string directory, string playerName, DateTime startTime, int recentLimit = 8){
        _recentLimit = recentLimit;
        Directory.CreateDirectory(directory); //jesli nie istnieje to tworzymy katalog
        string safeName = MakeSafeFileName(playerName); //usuwamy niedozwolone znaki 
        string timestamp = startTime.ToString("yyyyMMdd_HHmmss"); 
        FilePath = Path.Combine(directory, $"{safeName}_{timestamp}.log"); //tworzymy unique filepath zeby nie overwritowac!
        using var stream = new FileStream(FilePath, FileMode.CreateNew); //tworzymy pusty plik
    }

    //jeśli istnieje → wyjątek, więc nie nadpisujesz logów 

    public void Add(string message){ //dodaje event do loga
        string line = $"[{DateTime.Now:HH:mm:ss}] {message}";
        File.AppendAllLines(FilePath, new[] { line }); 
        _recentEntries.Enqueue(line); //dodajemy do kolejki
        while (_recentEntries.Count > _recentLimit) 
            _recentEntries.Dequeue(); //usuwa najstarsze wpisy!!
    }

    public IReadOnlyList<string> GetRecentEntries(){
        return _recentEntries.ToList();
    }

    public IReadOnlyList<string> GetAllEntries(){
        return File.ReadAllLines(FilePath);
    }

    private static string MakeSafeFileName(string text){ //zmieniamy niedozwolone znaki na _
        foreach (char c in Path.GetInvalidFileNameChars())
            text = text.Replace(c, '_');
        return text;
    }
}