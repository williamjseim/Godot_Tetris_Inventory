using Godot;

public class ItemData : ItemRef{
    public ItemData(Vector2I gridRef) : base(gridRef)
    {
        
    }

    public ItemSlot Itemslot { get; set; }
    public ItemHolder itemHolder { get; set; }
}

public class ItemRef{
    
    public ItemRef(Vector2I gridref)
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