using System;
using Godot;

public partial class ItemSlot : Panel{

    public ItemSlot()
    {
        this.MouseFilter = MouseFilterEnum.Pass;
        this.AddThemeStyleboxOverride("panel", itemsprite);
    }

    private Vector2I _gridPosition;
    public Vector2I GridPosition
    {
        get { return _gridPosition; }
        set {
            _gridPosition = value;
            this.Position = value * 64;
        }
    }
    
    public Vector2I SlotSize { get { return (Vector2I)this.Size / 64; } set { this.Size = value * 64; } }
    private ItemHolder _ItemHolder;
    public ItemHolder ItemHolder
    {
        get { return _ItemHolder; }
        set { _ItemHolder = value;
            if(value != null){

                this.itemsprite.Texture = ItemHolder.Texture;
                this.SlotSize = value.Item.ItemSize;
                this.TreeExited += ()=>{ this._ItemHolder.ItemRemoved(); };
            }
        }
    }

    StyleBoxTexture itemsprite = new StyleBoxTexture();
    public override void _Ready()
    {
        base._Ready();
        this.GuiInput += this.Input;
    }

    bool isDragged = false;
    public static event Action<ItemSlot> DragBegin;
    public static event Action<ItemSlot> DragEnd;

    public override void _Process(double delta)
    {
        base._Process(delta);
        if(isDragged){
            this.GlobalPosition = this.GetGlobalMousePosition() - this.Size / 2;
        }
    }

    protected void Input(InputEvent @event){
        if(@event is InputEventMouse mouse){
            if(mouse.ButtonMask == MouseButtonMask.Left && mouse.IsPressed()){
                if(isDragged){
                    isDragged = false;
                    DragEnd?.Invoke(this);
                    TopLevel = true;
                    return;
                }
                isDragged = true;
                TopLevel = true;
                DragBegin?.Invoke(this);
                // this.MouseFilter = MouseFilterEnum.Ignore;
            }
        }
    }
}