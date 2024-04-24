using Godot;
using System;
using System.Collections.Generic;

public partial class InventoryManager : ContainerManager
{
    List<IUi> Uis = new();
    public static PackedScene containerScene { get; protected set;} = ResourceLoader.Load<PackedScene>("res://Inventory/Scenes/Slot.tscn");
    public static int SlotSize = 64;
    [Export] SlotContainer slotContainer;
    SlotContainer FocusedContainer;
    public override void _Ready()
    {
        ItemDatabase.Instance.LoadItems();
        base._Ready();
        SlotContainer.MouseEnteredContainer += (e)=>{ FocusedContainer = e; };
        SlotContainer.MouseLeftContainer += (e)=>{ FocusedContainer = null; };
        this.InsertItem(ItemDatabase.Instance.GetItem("BasicRod"), slotContainer);
    }

    public void InsertItem(BaseItem item, SlotContainer container){
        container.InsertItem(new ItemHolder(){Item = item, Amount = 1});
    }

}
