using Godot;
using System;

public partial class InventoryManager : ContainerManager
{
    public override void _Ready()
    {
        base._Ready();
        ItemDatabase.Instance.LoadItems();
        BaseItem item = ItemDatabase.Instance.GetItem($"BasicRod");
        InsertItem(item);
    }

    public bool InsertItem(BaseItem item){
        for (int y = 0; y < this.inventorySize.Y; y++)
        {
            for (int x = 0; x < this.inventorySize.X; x++)
            {
                if(ItemFits(new Vector2I(x,y), item.ItemSize)){
                    
                }
            }
        }
        return false;
    }

    public bool ItemFits(Vector2I pos, Vector2I itemSize){
        for (int Y = pos.X; Y < itemSize.Y + pos.X; Y++)
        {
            for (var X = pos.X; X < itemSize.X + pos.X; X++)
            {
                if(!this.Slots[X,Y].IsEmpty){
                    return false;
                }
                return true;
            }
        }
        return false;
    }

    public bool PlaceItem(BaseItem item, Vector2I pos){
        ItemSlot slot = new ItemSlot();
        slot.ItemHolder = new ItemHolder(item, 1);
        this.container.AddChild(slot);
        return false;
    }
}
