namespace Rpg;

public class Equipment{
    public Item? Left {get; private set;}
    public Item? Right {get; private set;}

    public void Clear(){
        Left = null;
        Right = null;
    }

    public void SetLeft(Item? item) => Left = item;
    public void SetRight(Item? item) => Right = item;

    public Item? RemoveLeft(){
        var item = Left; 
        if(item == null) return null; 
        if(ReferenceEquals(Left, Right)){ //sprawdzamy czy jest dwureczna bron 
            Left = null;
            Right = null;
            return item;
        }
        Left = null;
        return item;
    }

    public Item? RemoveRight()
    {
        var item = Right;
        if (item == null) return null;
        if (ReferenceEquals(Left, Right)){
            Left = null;
            Right = null;
            return item;
        }
        Right = null;
        return item;
    }
}