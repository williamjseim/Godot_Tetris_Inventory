using System;
using Godot;

/// <summary>
/// itemslot is the should just be the graphical resprisitation of items in the inventory
/// </summary>
public partial class ItemSlot : Panel, ISaveAble{

    public ItemSlot()
    {
        this.ZIndex = 1;
        MouseFilter = MouseFilterEnum.Ignore;
        // this.TopLevel = true;
    }

    [Export] Label amountLabel;
    [Export] HBoxContainer ModifierContainer;

    public Vector2 TruePosition { get{ return this.GlobalPosition + new Vector2(InventoryManager.SlotSize/2, InventoryManager.SlotSize/2); } }

    private Vector2I _gridPosition;
    public Vector2I GridPosition
    {
        get { return _gridPosition; }
        set {
            _gridPosition = value;
        }
    }

    private SlotContainer _container;
    public SlotContainer Container
    {
        get { return _container; }
        set { _container = value; }
    }
    
    public Vector2I SlotSize { get { return (Vector2I)this.Size / 64; } set { this.Size = value * 64; } }
    private ItemHolder _ItemHolder;
    public ItemHolder ItemHolder
    {
        get { return _ItemHolder; }
        set { _ItemHolder = value;
            if(!value.Equals(ItemHolder.Empty)){
                this.itemsprite.Texture = ItemHolder.Texture;
                this.SlotSize = value.ItemSize;
            }
        }
    }

    public bool IsFull
    {
        get { return ItemHolder.Amount >= ItemHolder.Item.StackSize; }
    }

    private bool _justRotated = false;
    public bool JustRotated { get { return _justRotated; } set { _justRotated = value; } }
    public bool Rotated { get { return _ItemHolder.Rotated;} set {
        _ItemHolder.Rotated = value;
        _justRotated = !_justRotated;
        if(value == true){
            this.itemsprite.Texture = this.ItemHolder.Item.RotatedItemTexture;
            this.SlotSize = this.ItemSize;
        }
        else{
            this.itemsprite.Texture = this.ItemHolder.Item.ItemTexture;
            this.SlotSize = this.ItemSize;
        }
    }}

    public Vector2I ItemSize { get { return ItemHolder.ItemSize;} }
    StyleBoxTexture itemsprite = new StyleBoxTexture();
    public override void _Ready()
    {
        base._Ready();
        this.AddThemeStyleboxOverride("panel", itemsprite);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public int CombineItems(ItemHolder item){
        int spaceAvailable = this.ItemHolder.Item.StackSize - this.ItemHolder.Amount;
        if(spaceAvailable > item.Amount){
            _ItemHolder.Amount += item.Amount;
            return 0;
        }
        else{
            int extras = Math.Abs(item.Amount - ItemHolder.Amount);
            _ItemHolder.Amount = this.ItemHolder.Item.StackSize;
            return extras;
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