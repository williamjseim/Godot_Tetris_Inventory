using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;

public partial class InventoryManager : ContainerManager, ISaveAble
{
    List<IUi> Uis = new();
    public static PackedScene containerScene { get; protected set;} = ResourceLoader.Load<PackedScene>("res://Inventory/Scenes/Slot.tscn");
    public static int SlotSize = 64;
    [Export] SlotContainer slotContainer;
    SlotContainer FocusedContainer;
    public Highlight HighlightPanel { get; protected set; }

    public bool Dragging { get; set; }
    public Vector2I DragPosition { get; set; }
    
    public ItemSlot focusedSlot;
    public override void _Ready()
    {
        base._Ready();
        if(HighlightPanel == null){
            HighlightPanel = new();
        }
        ItemDatabase.Instance.LoadItems();
        SlotContainer.MouseEnteredContainer += (e)=>{ FocusedContainer = e; };
        SlotContainer.MouseLeftContainer += (e)=>{ FocusedContainer = null; };
        SlotContainer.MouseMotion += MouseMotion;
        SlotContainer.MouseEnteredContainer += ContainerEnter;
        SlotContainer.MouseLeftContainer += ContainerExit;
        SlotContainer.MousePressed += MousePressed;
        SlotContainer.MouseReleased += MouseReleased;
        this.InsertItem(ItemDatabase.Instance.GetItem("BasicRod"), slotContainer);
        this.InsertItem(ItemDatabase.Instance.GetItem("Fish"), slotContainer);
        this.InsertItem(ItemDatabase.Instance.GetItem("InternalStorage"), slotContainer);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if(Input.IsActionJustPressed("Rotate")){
            if(focusedSlot != null){
                focusedSlot.Rotated = !focusedSlot.Rotated;
                var local = FocusedContainer.GetLocalMousePosition();
                focusedSlot.Position = local - focusedSlot.Size/2;
                HighlightPanel.Size = focusedSlot.Size;
                HighlightPanel.Position = FocusedContainer.MouseToSlotPosition(focusedSlot.TruePosition);
            }
        }
    }

    public void ContainerEnter(SlotContainer container){
        FocusedContainer = container;
        if(Dragging && !HighlightPanel.IsVisibleInTree() && HighlightPanel.GetParent() != container)
            container.AddChild(HighlightPanel);
    }

    public void ContainerExit(SlotContainer container){
        FocusedContainer = null;
        if(!Dragging && HighlightPanel.IsVisibleInTree() && HighlightPanel.GetParent() == container){
            container.RemoveChild(HighlightPanel);
        }
    }

    public void InsertItem(BaseItem item, SlotContainer container){
        container.InsertItem(new ItemHolder(){Item = item, Amount = 1});
    }

    public void MouseMotion(SlotContainer container, InputEventMouseMotion motion){
        Vector2I cellPosition = container.GetSlotIndex(motion.Position, focusedSlot == null ? Vector2I.Zero : focusedSlot.ItemSize);
        if(focusedSlot != null){
            focusedSlot.Position = motion.Position - focusedSlot.Size/2;
            if(Dragging){
                if(cellPosition != DragPosition){
                    Vector2I pos = container.GetSlotIndex(focusedSlot.TruePosition, focusedSlot.ItemSize);
                    Color color = container.ItemFits(pos, focusedSlot) ? Colors.Green : Colors.Red;
                    this.HighlightPanel.SetColor(color);
                    this.HighlightPanel.Position = container.GetSlotPosition(pos);
                    DragPosition = cellPosition;
                }
            }
        }
    }

    public void MousePressed(SlotContainer container, InputEventMouseButton button, ItemSlot itemslot){
        focusedSlot = itemslot;
        Dragging = true;
        container.AddChild(HighlightPanel);
        HighlightPanel.Size = itemslot.Size;
        HighlightPanel.Position = itemslot.Position;
    }

    public void MouseReleased(SlotContainer container, InputEventMouse mouse){
        Dragging = false;
        if(focusedSlot != null){
            if(!container.InsertItem(container.GetSlotIndex(focusedSlot.TruePosition, focusedSlot.ItemSize), focusedSlot)){
                focusedSlot.Container.ResetItem(focusedSlot);
            }
            focusedSlot.JustRotated = false;
            focusedSlot = null;
            container.RemoveChild(HighlightPanel);
        }

    }

    public object Save()
    {
        throw new NotImplementedException();
    }

    public void Load(object obj)
    {
        throw new NotImplementedException();
    }

}
