using Godot;

/// <summary>
/// this is just filler in the grid to tell that the object is 
/// </summary>
public class ItemDataRef : GridFiller{
    
    public ItemDataRef(Vector2I gridref, ItemSlot itemslot) : base(itemslot)
    {
        this._gridRef = gridref;
    }
    private Vector2I _gridRef;
    public Vector2I GridRef
    {
        get { return _gridRef; }
        set { _gridRef = value; }
    }
    
}