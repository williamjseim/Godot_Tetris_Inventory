using Godot;
using Newtonsoft.Json;

/// <summary>
/// this is just filler in the grid to tell that the object is 
/// </summary>
public class ItemFiller : IStorable{
    
    public ItemFiller(Vector2I gridref, ItemData data)
    {
        this._gridRef = gridref;
        this._data = data;
    }
    private Vector2I _gridRef;
    public Vector2I GridRef
    {
        get { return _gridRef; }
        set { _gridRef = value; }
    }
    private ItemData _data;
    public ItemData Data { get { return _data; } set { _data = value; } }

}