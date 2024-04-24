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
    private bool _justRotated = false;
    public bool JustRotated { get { return _justRotated; } set { _justRotated = value; } }
    private bool _rotated = false;
    public bool Rotated { get { return _rotated;} set {
        _rotated = value;
        _justRotated = true;
        if(value == true){
            this.itemsprite.Texture = this.ItemHolder.Item.RotatedItemTexture;
            this.SlotSize = this.ItemSize;
        }
        else{
            this.itemsprite.Texture = this.ItemHolder.Item.ItemTexture;
            this.SlotSize = this.ItemSize;
        }
    }}
    public Vector2I ItemSize { get { return Rotated ? new Vector2I(ItemHolder.Item.ItemSize.Y, ItemHolder.Item.ItemSize.X) : ItemHolder.Item.ItemSize; } }
    StyleBoxTexture itemsprite = new StyleBoxTexture();
    public override void _Ready()
    {
        base._Ready();
        this.AddThemeStyleboxOverride("panel", itemsprite);
    }

    public static event Action<ItemSlot> DragBegin;
    public static event Action<ItemSlot> DragEnd;

    public override void _Process(double delta)
    {
        base._Process(delta);
    }
}