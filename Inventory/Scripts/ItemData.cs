using System;
using Godot;

public class ItemData : GridFiller{
    public ItemData(ItemSlot itemslot) : base(itemslot)
    {
        
    }

    private ItemHolder _itemholder;
    public ItemHolder ItemHolder { get { return _itemholder; } set { _itemholder = value; } }

    public int CombineItems(ItemHolder item){
        return this.Itemslot.CombineItems(item);
    }
}