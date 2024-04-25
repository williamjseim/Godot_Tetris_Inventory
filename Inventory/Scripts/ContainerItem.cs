using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;

public class ContainerItem : BaseItem{

    public ContainerItem() : base()
    {
    }
    public Vector2I InternalStorageSize { set { GD.Print(value); this.Modifiers = new ItemModifier[]{ new ContainerModifier(value) }; } }
    
}