using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public partial class InventoryManager : Panel, ISaveAble
{
    [Export] PackedScene ContainerWindowScene;

    List<BaseWindow> Uis = new();
    public static PackedScene containerScene { get; protected set;} = ResourceLoader.Load<PackedScene>("res://Inventory/Scenes/Slot.tscn");
    public static int SlotSize = 64;
    [Export] SlotContainer slotContainer;
    SlotContainer FocusedContainer;
    BaseWindow FocusedWindow;
    public bool Dragging { get; set; }

    List<ContainerWindow> OpenedWindows = new(10);
    
    private ItemSlot _focusedSlot;
    public ItemSlot focusedSlot
    {
        get { return _focusedSlot; }
        set {
            if(_focusedSlot != null){
                _focusedSlot.TopLevel = false;
            }
            else{
                value.TopLevel = true;
            }
            _focusedSlot = value;
            
        }
    }
    
    public override void _Ready()
    {
        base._Ready();
        ItemDatabase.Instance.LoadItems();
        SlotContainer.MouseEnteredContainer += (e)=>{ FocusedContainer = e; };
        SlotContainer.MouseLeftContainer += (e)=>{ FocusedContainer = null; };
        SlotContainer.MouseMotion += MouseMotion;
        SlotContainer.MouseEnteredContainer += ContainerEnter;
        SlotContainer.MouseLeftContainer += ContainerExit;
        SlotContainer.MousePressed += MousePressed;
        SlotContainer.MouseReleased += MouseReleased;
        SlotContainer.MouseDoubleClick += this.DoubleClick;
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
                var local = GetGlobalMousePosition();
                focusedSlot.Position = local - focusedSlot.Size/2;
            }
        }
    }

    public void ContainerEnter(SlotContainer container){
        if(FocusedContainer != container){
            FocusedContainer = container;
            focusedSlot.Container.RemoveChild(focusedSlot);
            container.AddChild(focusedSlot);
        }
    }

    public void ContainerExit(SlotContainer container){
        FocusedContainer = null;
    }

    public void InsertItem(BaseItem item, SlotContainer container){
        container.InsertItem(new ItemHolder(item, 1));
    }

    public void MouseMotion(InputEventMouseMotion motion){
        if(focusedSlot != null){
            focusedSlot.Position = motion.GlobalPosition - focusedSlot.Size/2;
            if(Dragging){
                if(FocusedContainer != null){
                    // Vector2I cellPosition = FocusedContainer.GetSlotIndex(motion.Position, focusedSlot == null ? Vector2I.Zero : focusedSlot.ItemSize);
                    FocusedContainer.Highlight(focusedSlot);
                }
            }
        }
    }

    public void MousePressed(SlotContainer container, InputEventMouseButton button, ItemSlot itemslot){
        focusedSlot = itemslot;
        Dragging = true;
    }

    public void MouseReleased(SlotContainer container, InputEventMouse mouse){
        
        if(focusedSlot != null){
            if(FocusedContainer == null){
                this.ResetItemPosition(focusedSlot);
                focusedSlot = null;
                return;
            }
            if(!FocusedContainer.InsertItem(FocusedContainer.GlobalToSlotIndex(focusedSlot.TruePosition, focusedSlot.ItemSize), focusedSlot)){
                focusedSlot.Container.ResetItem(focusedSlot);
            }
            focusedSlot.JustRotated = false;
            focusedSlot = null;
        }
        Dragging = false;

    }

    public void DoubleClick(SlotContainer container, InputEventMouseButton button, ItemSlot item){
        if(item.ItemHolder.Item is ContainerItem){
            BaseWindow window = this.ContainerWindowScene.Instantiate<ContainerWindow>();
            this.AddChild(window);
            window.Itemslot = item;
            this.Uis.Add(window);
            window.Position = (this.Size / 2 - window.Size) + new Vector2(20, 20) * this.Uis.Count;
        }
    }

    public void ResetItemPosition(ItemSlot item){
        item.Container.ResetItem(item);
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
