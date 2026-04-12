namespace Rpg;

public class Inventory{
    private readonly List<Item> _items = new(); 
    public IReadOnlyList<Item> Items => _items; 
    public int Count => _items.Count;
    public void Add(Item item) => _items.Add(item);
    public Item RemoveAt(int index) {
        var item = _items[index];
        _items.RemoveAt(index);
        return item;
    }
}