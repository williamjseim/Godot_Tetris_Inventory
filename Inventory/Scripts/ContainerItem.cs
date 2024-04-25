using System.Collections.Generic;
using Godot;

public class ContainerItem : BaseItem{

    public ContainerItem() : base()
    {
        this.Modifiers = new ItemModifier[4];
    }
    public Vector2I InternalStorageSize { get; protected set; } = Vector2I.One;
    
}