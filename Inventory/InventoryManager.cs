using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

public partial class InventoryManager : ContainerManager, ISaveAble
{
    [Export] PackedScene ContainerWindowScene;
    public static PackedScene ItemslotScene { get; protected set;} = ResourceLoader.Load<PackedScene>("res://Inventory/Scenes/Itemslot.tscn");
    public static int SlotSize = 64;
    [Export] SlotContainer slotContainer;
    SlotContainer _focusedContainer;
    BaseWindow FocusedWindow;
    Vector2 windowPos;
    private bool _dragging;
    public bool Dragging { get { return _dragging; } set {
        _dragging = value;
        if(_focusedSlot != null){
            if(Dragging){
                _focusedSlot.TopLevel = true;
                _focusedSlot.JustRotated = false;
                _focusedSlot.GlobalPosition = (GetLocalMousePosition() - _focusedSlot.Size/2);
            }
            else{
                _focusedSlot.TopLevel = false;
            }
        }
    }}
    LinkedList<BaseWindow> OpenedWindows = new();
    
    private ItemSlot _focusedSlot;
    public ItemSlot focusedSlot
    {
        get { return _focusedSlot; }
        set {
            if(_focusedSlot != null){
                _focusedSlot.TopLevel = false;
            }
            _focusedSlot = value;
        }
    }
    
    public override void _Ready()
    {
        base._Ready();
        ItemDatabase.Instance.LoadItems();
        ContainerManager.MouseMotion += MouseMotion;
        SlotContainer.MouseEnteredContainer += (e)=>{ _focusedContainer = e; };
        SlotContainer.MouseLeftContainer += (e)=>{ _focusedContainer = null; };
        SlotContainer.MouseEnteredContainer += ContainerEnter;
        SlotContainer.MouseLeftContainer += ContainerExit;
        SlotContainer.MousePressed += MousePressed;
        SlotContainer.MouseReleased += MouseReleased;
        SlotContainer.MouseDoubleClick += this.DoubleClick;
        BaseWindow.Pressed += WindowPressed;
        BaseWindow.Released += WindowReleased;
        BaseWindow.CloseEvent += WindowClosed;
        // this.InsertItem(ItemDatabase.Instance.GetItem("BasicRod"), slotContainer);
        // this.InsertItem(ItemDatabase.Instance.GetItem("Fish"), slotContainer);
        // this.InsertItem(ItemDatabase.Instance.GetItem("TackleBox"), slotContainer);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if(Input.IsActionJustPressed("Rotate")){
            if(focusedSlot != null){
                focusedSlot.Rotated = !focusedSlot.Rotated;
                var local = GetGlobalMousePosition();
                focusedSlot.Position = local - focusedSlot.Size/2;
                _focusedContainer.Highlight(focusedSlot);
            }
        }
        if(Input.IsActionJustPressed("Close Ui")){
            if(OpenedWindows.Count > 0){
                OpenedWindows.Last().Close();
                OpenedWindows.RemoveLast();
            }
        }
        if(Input.IsActionJustPressed("Debug")){
            // this.Save();
            this.Load(null);
        }
    }

    public void WindowPressed(BaseWindow window, InputEventMouse mouse){
        if(OpenedWindows.Last() != window){
            OpenedWindows.Remove(window);
            OpenedWindows.AddLast(window);
        }
        FocusedWindow = window;
        windowPos = mouse.Position;
    }

    public void WindowReleased(BaseWindow window, InputEventMouse mouse){
        FocusedWindow = null;
    }

    public void ContainerEnter(SlotContainer container){
        if(_focusedContainer != container){
            _focusedContainer = container;
        }
    }

    public void ContainerExit(SlotContainer container){
        _focusedContainer = null;
    }

    public void InsertItem(BaseItem item, SlotContainer container){
        container.InsertItem(new ItemHolder(item, 1));
    }
    public new void MouseMotion(InputEventMouseMotion motion){
        if(focusedSlot != null){
            Dragging = true;
            if(Dragging){
                focusedSlot.Position = motion.GlobalPosition - focusedSlot.Size/2;
                if(_focusedContainer != null){
                    _focusedContainer.Highlight(focusedSlot);
                }
            }
        }
        else if(FocusedWindow != null){
            FocusedWindow.GlobalPosition = motion.GlobalPosition - windowPos;
        }
    }

    public void MousePressed(SlotContainer container, InputEventMouseButton button, ItemSlot itemslot){
        focusedSlot = itemslot;
    }

    public void MouseReleased(SlotContainer container, InputEventMouse mouse){
        if(focusedSlot != null){
            if(_focusedContainer == null){
                this.ResetItemPosition(focusedSlot);
            }
            else if(!_focusedContainer.InsertItem(_focusedContainer.GlobalToSlotIndex(focusedSlot.TruePosition, focusedSlot.ItemSize), focusedSlot)){
                focusedSlot.Container.ResetItem(focusedSlot);
            }
            focusedSlot = null;
        }
        Dragging = false;
    }

    public void DoubleClick(SlotContainer container, InputEventMouseButton button, ItemSlot item){
        if(item.ItemHolder.Item is ContainerItem containeritem && !this.OpenedWindows.Where(i=>i.Itemslot == item).Any()){
            BaseWindow window = this.ContainerWindowScene.Instantiate<ContainerWindow>();
            this.AddChild(window);
            window.Itemslot = item;
            this.OpenedWindows.AddLast(window);
            window.Position = (this.Size / 2 - window.Size) + new Vector2(20, 20) * this.OpenedWindows.Count;
        }
    }

    public void WindowClosed(BaseWindow window){
        this.OpenedWindows.Remove(window);
    }

    public void ResetItemPosition(ItemSlot item){
        item.Container.ResetItem(item);
    }    

    public object Save()
    {
        List<ItemData> states = new();
        foreach (var item in this.slotContainer.Slots)
        {
            if(item is ItemData data){
                states.Add(data);
            }
        }
        SaveLoad.Save(states);
        return states;
    }

    public void Load(object obj)
    {
        var list = SaveLoad.Load();
        foreach (var item in list){
            if(item.ItemHolder.TryGetModifier<ContainerModifier>(out ContainerModifier modifier)){
                foreach (var data in modifier.Grid)
                {
                    GD.Print(data);
                }
            }
            this.slotContainer.InsertItem(item);
        }
    }

}
