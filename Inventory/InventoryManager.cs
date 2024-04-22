using Godot;
using System;

public partial class InventoryManager : ContainerManager
{
    public override void _Ready()
    {
        base._Ready();
        ItemDatabase.Instance.LoadItems();
        BaseItem item = ItemDatabase.Instance.GetItem($"BasicRod");
        ItemSlot.DragBegin += (s)=>{ itemSlotBeingDragged = s; };
        ItemSlot.DragEnd += (s)=>{ itemSlotBeingDragged = null; };
        Slot.MouseEnter += (gridPos)=>{ 
            if(itemSlotBeingDragged != null){
                HighLightSlots(gridPos, itemSlotBeingDragged);
            }
        };
        Slot.MouseExit += (gridPos)=>{ 
            if(itemSlotBeingDragged != null){
                DeHighlightSlots(gridPos, itemSlotBeingDragged);
            }
        };
        InsertItem(item);
    }

    ItemSlot itemSlotBeingDragged;

    public override void _Process(double delta)
    {
        if(Input.IsKeyPressed(Key.H)){
            BaseItem item = ItemDatabase.Instance.GetItem($"BasicRod");
            InsertItem(item);
        }
    }

    Vector2I GetTruePosition(Vector2I gridPos, BaseItem item){
        int x = Math.Clamp(gridPos.X - item.ItemSize.X / 2, 0, inventorySize.X - item.ItemSize.X);
        int y = Math.Clamp(gridPos.Y - item.ItemSize.Y / 2, 0, inventorySize.Y - item.ItemSize.Y);
        Vector2I truePos = new Vector2I(x, y);
        return truePos;
    }

    public void HighLightSlots(Vector2I pos, ItemSlot slot){
        Vector2I truePos = GetTruePosition(pos, slot.ItemHolder.Item);
        for (int y = truePos.Y; y < truePos.Y+slot.ItemHolder.Item.ItemSize.Y; y++)
        {
            for (int x = truePos.X; x < truePos.X+slot.ItemHolder.Item.ItemSize.X; x++)
            {
                Slots[x,y].HighLight();
            }
        }
    }

    public void DeHighlightSlots(Vector2I pos, ItemSlot slot){
        Vector2I truePos = GetTruePosition(pos, slot.ItemHolder.Item);
        for (int y = truePos.Y; y < truePos.Y+slot.ItemHolder.Item.ItemSize.Y; y++)
        {
            for (int x = truePos.X; x < truePos.X+slot.ItemHolder.Item.ItemSize.X; x++)
            {
                Slots[x,y].DeHighLight();
            }
        }
    }

    public bool InsertItem(BaseItem item){
        for (int y = 0; y < this.inventorySize.Y; y++)
        {
            for (int x = 0; x < this.inventorySize.X; x++)
            {
                if(ItemFits(new Vector2I(x,y), item.ItemSize)){
                    PlaceItem(item, 1, new Vector2I(x,y));
                    return true;
                }
            }
        }
        return false;
    }

    public bool ItemFits(Vector2I pos, Vector2I itemSize){
        for (int Y = pos.Y; Y < itemSize.Y + pos.Y; Y++)
        {
            for (var X = pos.X; X < itemSize.X + pos.X; X++)
            {
                if(!this.Slots[X,Y].IsEmpty){
                    return false;
                }
            }
        }
        return true;
    }

    public bool PlaceItem(BaseItem item, int amount, Vector2I pos){
        ItemSlot slot = new ItemSlot();
        slot.ItemHolder = new ItemHolder(item, amount);
        slot.GridPosition = pos;
        this.container.AddChild(slot);
        GD.Print(pos);
        for (int y = pos.Y; y < pos.Y+item.ItemSize.Y; y++)
        {
            for (int x = pos.X; x < pos.X+item.ItemSize.X; x++)
            {
                GD.Print("slot item", "x", x, "y", y);
                Slots[x, y].ItemSlot = slot;
            }
        }
        return false;
    }
}
