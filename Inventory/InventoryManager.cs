using Godot;
using System;

public partial class InventoryManager : ContainerManager
{
    [Export] PackedScene itemslotScene;
    public override void _Ready()
    {
        base._Ready();
        ItemDatabase.Instance.LoadItems();
        ItemSlot.DragBegin += (s)=>{ 
            _itemSlotBeingDragged = s;
            RemoveItem(s.GridPosition, s.ItemSize);
            HighLightSlots(GetTruePosition(focusedSlot.GridPosition, _itemSlotBeingDragged), s);
        };
        ItemSlot.DragEnd += (s)=>{ _itemSlotBeingDragged = null; };
        Slot.Clicked += DropItem;
        Slot.MouseEnter += (slot)=>{
            if(_itemSlotBeingDragged != null){
                this.focusedSlot = slot;
                HighLightSlots(GetTruePosition(slot.GridPosition, _itemSlotBeingDragged), _itemSlotBeingDragged);
            }
        };
        Slot.MouseExit += (slot)=>{ 
            if(_itemSlotBeingDragged != null){
                focusedSlot = null;
                DeHighlightSlots(GetTruePosition(slot.GridPosition, _itemSlotBeingDragged), _itemSlotBeingDragged);
            }
        };
        InsertItem(ItemDatabase.Instance.GetItem($"BasicRod"));
        InsertItem(ItemDatabase.Instance.GetItem($"Fish"));
        InsertItem(ItemDatabase.Instance.GetItem($"InternalStorage"));
    }


    Slot focusedSlot = null;
    ItemSlot _itemSlotBeingDragged;

    public override void _Process(double delta)
    {
        if(Input.IsKeyPressed(Key.H)){
            BaseItem item = ItemDatabase.Instance.GetItem($"BasicRod");
            InsertItem(item);
        }
        if (Input.IsActionJustPressed("Rotate"))
        {
            if(_itemSlotBeingDragged != null){
                DeHighlightSlots(GetTruePosition(focusedSlot.GridPosition, _itemSlotBeingDragged), _itemSlotBeingDragged);
                _itemSlotBeingDragged.Rotated = !_itemSlotBeingDragged.Rotated;
            }
        }
    }

    Vector2I GetTruePosition(Vector2I gridPos, ItemSlot item){
        int x = Math.Clamp(gridPos.X - item.ItemSize.X / 2, 0, inventorySize.X - item.ItemSize.X);
        int y = Math.Clamp(gridPos.Y - item.ItemSize.Y / 2, 0, inventorySize.Y - item.ItemSize.Y);
        Vector2I truePos = new Vector2I(x, y);
        return truePos;
    }

    public void HighLightSlots(Vector2I truePos, ItemSlot slot){
        bool fits = ItemFits(truePos, slot.ItemSize);
        for (int y = truePos.Y; y < truePos.Y+slot.ItemSize.Y; y++)
        {
            for (int x = truePos.X; x < truePos.X+slot.ItemSize.X; x++)
            {
                try{
                    Slots[x,y].HighLight(fits ? Colors.Green : Colors.Red);
                }
                catch( IndexOutOfRangeException ex) { GD.PrintErr(x,y);}
            }
        }
    }

    public void DeHighlightSlots(Vector2I truePos, ItemSlot slot){
        for (int y = truePos.Y; y < truePos.Y+slot.ItemSize.Y; y++)
        {
            for (int x = truePos.X; x < truePos.X+slot.ItemSize.X; x++)
            {
                Slots[x,y].DeHighLight(Slots[x,y].IsEmpty ? Colors.Transparent : Colors.Purple);
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
        ItemSlot slot = itemslotScene.Instantiate<ItemSlot>();
        slot.ItemHolder = new ItemHolder(item, amount);
        slot.GridPosition = pos;
        this.container.AddChild(slot);
        slot.Position = pos * 64;
        GD.Print(Slots[pos.X,pos.Y].Position, pos);
        for (int y = pos.Y; y < pos.Y+item.ItemSize.Y; y++)
        {
            for (int x = pos.X; x < pos.X+item.ItemSize.X; x++)
            {
                Slots[x, y].ItemSlot = slot;
                Slots[x, y].HighLight(Colors.Purple);
            }
        }
        return false;
    }

    public void RemoveItem(Vector2I pos, Vector2I itemSize){
        for (int y = pos.Y; y < pos.Y + itemSize.Y; y++)
        {
            for (int x = pos.X; x < pos.X + itemSize.X; x++)
            {
                Slots[x,y].ItemSlot = null;
            }
        }
    }

    public void DropItem(Slot slot){
        if(_itemSlotBeingDragged != null){
            Vector2I truePos = GetTruePosition(slot.GridPosition, _itemSlotBeingDragged);
            Vector2I itemSize = _itemSlotBeingDragged.ItemSize;
            if(ItemFits(truePos, itemSize)){
                RemoveItem(_itemSlotBeingDragged.GridPosition, itemSize);
                DeHighlightSlots(GetTruePosition(slot.GridPosition, _itemSlotBeingDragged), _itemSlotBeingDragged);
                _itemSlotBeingDragged.GridPosition = truePos;
                _itemSlotBeingDragged.StopDrag();
                _itemSlotBeingDragged.GlobalPosition = Slots[truePos.X, truePos.Y].GlobalPosition;
                GD.Print("global", Slots[truePos.X, truePos.Y].GlobalPosition);
                for (int y = truePos.Y; y < truePos.Y + itemSize.Y; y++)
                {
                    for (int x = truePos.X; x < truePos.X + itemSize.X; x++)
                    {
                        Slots[x,y].ItemSlot = _itemSlotBeingDragged;
                        Slots[x,y].HighLight(Colors.Purple);
                    }
                }
                _itemSlotBeingDragged = null;
            }
        }
    }
}
