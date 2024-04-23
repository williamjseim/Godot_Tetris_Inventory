using System;
using Godot;

public partial class ItemSlot : Panel{

    private Vector2I _gridPosition;
    public Vector2I GridPosition
    {
        get { return _gridPosition; }
        set {
            _gridPosition = value;
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

    private bool _rotated = false;
    public bool Rotated { get { return _rotated;} set {
        _rotated = value;
        if(value == true){
            Image image = this.itemsprite.Texture.GetImage();
            image.Rotate90(ClockDirection.Clockwise);
            this.itemsprite.Texture = ImageTexture.CreateFromImage(image);
            this.SlotSize = this.ItemSize;
        }
        else{
            this.itemsprite.Texture = this.ItemHolder.Item.ItemSprite;
            this.SlotSize = this.ItemSize;
        }
    }}
    public Vector2I ItemSize { get { return Rotated ? new Vector2I(ItemHolder.Item.ItemSize.Y, ItemHolder.Item.ItemSize.X) : ItemHolder.Item.ItemSize; } }

    StyleBoxTexture itemsprite = new StyleBoxTexture();
    public override void _Ready()
    {
        base._Ready();
        this.AddThemeStyleboxOverride("panel", itemsprite);
        this.GuiInput += this.Input;
    }

    bool isDragged = false;
    public static event Action<ItemSlot> DragBegin;
    public static event Action<ItemSlot> DragEnd;

    public void StopDrag(){
        isDragged = false;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if(isDragged){
            this.GlobalPosition = this.GetGlobalMousePosition() - this.Size / 2;
        }
    }

    public void Input(InputEvent @event){
        if(@event is InputEventMouse mouse){
            if(mouse.ButtonMask == MouseButtonMask.Left && mouse.IsPressed()){
                this.GlobalPosition = this.GetGlobalMousePosition() - this.Size / 2;
                isDragged = true;
                TopLevel = true;
                DragBegin?.Invoke(this);
                // this.MouseFilter = MouseFilterEnum.Ignore;
            }
        }
    }
}