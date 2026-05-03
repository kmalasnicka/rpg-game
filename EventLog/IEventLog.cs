namespace Rpg;

public interface IEventLog{
    string FilePath { get; }
    void Add(string message); 
    IReadOnlyList<string> GetRecentEntries(); //ostatnie wpisy
    IReadOnlyList<string> GetAllEntries(); //wszystkie wpisy
}