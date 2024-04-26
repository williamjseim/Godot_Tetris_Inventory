using Godot;

public abstract partial class BaseWindow : Panel{
	[Export] public Button CloseButton;

    public override void _Ready()
    {
        base._Ready();
        this.CloseButton.ButtonDown += Close;
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
    
    private ItemModifier[] _itemModifiers;
    public ItemModifier[] ItemModifiers
    {
        get { return _itemModifiers; }
        set { 
            _itemModifiers = value; 
            this.SetupModifiers(value);
        }
    }

    public abstract void Close();

    public abstract void SetupModifiers(ItemModifier[] modifiers);
    
}