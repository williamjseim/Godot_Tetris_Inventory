using Godot;
using System;
using System.Collections.Generic;

public partial class InventoryManager : ContainerManager
{
    List<IUi> Uis = new();
    [Export] PackedScene itemslotScene;
    [Export] PackedScene containerScene;
    public override void _Ready()
    {
        base._Ready();
        ItemDatabase.Instance.LoadItems();
        ItemSlot.DragBegin += (s)=>{ 
            _itemSlotBeingDragged = s;
            RemoveItem(s.GridPosition, s);
            HighLightSlots(GetTruePosition(focusedSlot.GridPosition, s), s);
        };
        ItemSlot.DragEnd += (s)=>{ _itemSlotBeingDragged = null; };
        Slot.Clicked += Clicked;
        Slot.Released += Released;
        Slot.DoubleClick += DoubleClick;
        Slot.MouseEnter += (slot)=>{
            if(_itemSlotBeingDragged != null){
                this.focusedSlot = slot;
                drag = true;
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
    bool drag = false;
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
                drag = true;
            }
        }
        if(_itemSlotBeingDragged != null && drag){
            _itemSlotBeingDragged.GlobalPosition = GetGlobalMousePosition() - _itemSlotBeingDragged.Size / 2;
        }
    }

    Vector2I GetTruePosition(Vector2I gridPos, ItemSlot item){
        int x = Math.Clamp(gridPos.X - item.ItemSize.X / 2, 0, inventorySize.X - item.ItemSize.X);
        int y = Math.Clamp(gridPos.Y - item.ItemSize.Y / 2, 0, inventorySize.Y - item.ItemSize.Y);
        Vector2I truePos = new Vector2I(x, y);
        return truePos;
    }

    private void Clicked(InputEventMouse mouse, Slot sender){
        if(sender.ItemSlot != null){
            this._itemSlotBeingDragged = sender.ItemSlot;
        }
    }

    private void DoubleClick(InputEventMouse mouse, Slot sender){
        if(sender.ItemSlot.ItemHolder.Item is ContainerItem item){

        }
    }

    public void Released(InputEventMouse mouse, Slot sender){
        if(_itemSlotBeingDragged != null){
            if(DropItem(focusedSlot, _itemSlotBeingDragged)){
                drag = false;
                _itemSlotBeingDragged.JustRotated = false;
                _itemSlotBeingDragged = null;
                return;
            }
        }
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
                catch { GD.PrintErr(x,y);}
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
                // GD.Print(Slots[X,Y].IsEmpty, " empty", X,Y);
                if(!this.Slots[X,Y].IsEmpty && this.Slots[X,Y].ItemSlot != _itemSlotBeingDragged){
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

    public void RemoveItem(Vector2I pos, ItemSlot itemSlot){
        Vector2I trueSize;
        if(itemSlot.JustRotated){
            GD.Print("is rotated");
            trueSize = new Vector2I(itemSlot.ItemSize.Y, itemSlot.ItemSize.X);
        }
        else{
            trueSize = itemSlot.ItemSize;
        }
        for (int y = pos.Y; y < pos.Y + trueSize.Y; y++)
        {
            for (int x = pos.X; x < pos.X + trueSize.X; x++)
            {
                Slots[x,y].ItemSlot = null;
                Slots[x,y].DeHighLight();
            }
        }
    }

    public bool DropItem(Slot slot, ItemSlot itemSlot){
        if(itemSlot != null){
            Vector2I truePos = GetTruePosition(slot.GridPosition, _itemSlotBeingDragged);
            Vector2I itemSize = itemSlot.ItemSize;
            if(!ItemFits(truePos, itemSize)){
                truePos = itemSlot.GridPosition;
                if(itemSlot.JustRotated){
                    itemSlot.Rotated = !itemSlot.Rotated;
                }
            }
            else{
                RemoveItem(itemSlot.GridPosition, itemSlot);
                for (int y = truePos.Y; y < truePos.Y + itemSize.Y; y++)
                {
                    for (int x = truePos.X; x < truePos.X + itemSize.X; x++)
                    {
                        Slots[x,y].ItemSlot = itemSlot;
                        Slots[x,y].HighLight(Colors.Purple);
                    }
                }
            }
            DeHighlightSlots(GetTruePosition(slot.GridPosition, itemSlot), itemSlot);   
            DeHighlightSlots(itemSlot.GridPosition, itemSlot);   
            itemSlot.GridPosition = truePos;
            itemSlot.GlobalPosition = Slots[truePos.X, truePos.Y].GlobalPosition;
            itemSlot.JustRotated = false;
            return true;
        }
        return false;
    }
}
