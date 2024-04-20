using Godot;

public partial class ItemSlot : Panel{
    public Vector2I SlotSize { get { return (Vector2I)this.Size / 64; } set { this.Size = value * 64; } }
    private ItemHolder _ItemHolder;
    public ItemHolder ItemHolder
    {
        get { return _ItemHolder; }
        set { _ItemHolder = value;
            this.itemsprite.Texture = ItemHolder.Texture;
            this.SlotSize = value.Item.ItemSize;
        }
    }

    StyleBoxTexture itemsprite = new StyleBoxTexture();
    public override void _Ready()
    {
        base._Ready();
        this.AddThemeStyleboxOverride("panel", itemsprite);
    }

}