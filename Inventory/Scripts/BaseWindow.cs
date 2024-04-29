using System;
using System.Collections.Generic;
using Godot;

public abstract partial class BaseWindow : Panel{
    [Export] Panel Topbar;
 	[Export] public Button CloseButton;

    public static event Action<BaseWindow, InputEventMouse> Pressed;
    public static event Action<BaseWindow, InputEventMouse> Released;
    public static event Action<BaseWindow> CloseEvent;

    public override void _Ready()
    {
        base._Ready();
        this.CloseButton.ButtonDown += Close;
        this.GuiInput += Input;
    }

    private ItemSlot _itemslot;
    public ItemSlot Itemslot
    {
        get { return _itemslot; }
        set {
            _itemslot = value;
            ItemModifiers = value.ItemHolder.StaticModifiers;
        }
    }

    public void Input(InputEvent @event){
        if(@event is InputEventMouseButton mouse){
            if(mouse.ButtonMask == MouseButtonMask.Left && mouse.IsPressed()){
                Pressed.Invoke(this, mouse);
            }
            if(mouse.ButtonMask == MouseButtonMask.Left && mouse.IsReleased()){
                Pressed.Invoke(this, mouse);
            }
        }
    }
    
    private List<ItemModifier> _itemModifiers;
    public List<ItemModifier> ItemModifiers
    {
        get { return _itemModifiers; }
        set { 
            _itemModifiers = value; 
            this.SetupModifiers(value);
        }
    }

    public virtual void Close(){
        CloseEvent?.Invoke(this);
    }

    public abstract void SetupModifiers(List<ItemModifier> modifiers);
    
}