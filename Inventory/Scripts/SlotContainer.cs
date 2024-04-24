using System;
using Godot;

public partial class SlotContainer : Panel{

    private Slot[,] _slots;
    public Slot[,] Slots
    {
        get { return _slots; }
        set { _slots = value; }
    }
    [Export] public Vector2I ContainerSize { get; protected set; } = Vector2I.One;

    public static event Action<SlotContainer> MouseEnteredContainer;
    public static event Action<SlotContainer> MouseLeftContainer;
    public override void _Ready()
    {
        base._Ready();
        CustomMinimumSize = ContainerSize*InventoryManager.SlotSize;
        UpdateMinimumSize();
        MouseEntered += MouseEnter;
        MouseExited += MouseExit;
        this.GuiInput += Input;
        _slots = new Slot[ContainerSize.X, ContainerSize.Y];
         for (int y = 0; y < ContainerSize.Y; y++)
        {
            for (int x = 0; x < ContainerSize.X; x++)
            {
                _slots[x,y] = new(new(x,y));
            }
        }

    }
    Vector2I slot;
    Panel _panel;
    private void MouseEnter(){
        MouseEnteredContainer?.Invoke(this);
        if(_panel == null){
            _panel = new();
        StyleBoxFlat style = new();
        style.BgColor = Colors.Green;
        _panel.AddThemeStyleboxOverride("panel", style);
        _panel.Size = new Vector2(64, 64);
        _panel.MouseFilter = MouseFilterEnum.Ignore;
        }
        this.AddChild(_panel);
    }

    private void MouseExit(){
        MouseEnteredContainer?.Invoke(this);
        this.RemoveChild(_panel);
        _panel = null;
    }

    public override void _Draw()
    {
        base._Draw();
        for (int y = 0; y < ContainerSize.Y; y++)
        {
            for (int x = 0; x < ContainerSize.X; x++)
            {
                var rect = new Rect2(new Vector2I(x,y) * InventoryManager.SlotSize, Vector2.One * InventoryManager.SlotSize);
                DrawRect(rect, Colors.White, false);
            }
        }
    }
    public void Input(InputEvent @event){
        if(@event is InputEventMouseMotion motion){
            if(_panel != null){
                Vector2I slotPos = GetSlotPos();
                if(this.slot != slotPos){
                    this.slot = slotPos;
                    _panel.Position = slotPos*64;
                }
            }
        }
    }

    public Vector2I GetSlotPos(){
        Vector2I slotPos = (Vector2I)(GetLocalMousePosition() / 64);
        return slotPos;
    }

    public bool InsertItem(ItemHolder item){
        for (int y = 0; y <= ContainerSize.Y - item.Item.ItemSize.Y; y++)
        {
            for (int x = 0; x <= ContainerSize.X - item.Item.ItemSize.X; x++)
            {
                if(ItemFits(new Vector2I(x, y), item.Item)){
                    ItemSlot slot = new();
                    slot.GridPosition = new(x, y);
                    slot.ItemHolder = item.Clone();
                    slot.Size = item.Item.ItemSize * InventoryManager.SlotSize;
                    slot.Position = new Vector2I(x, y) * InventoryManager.SlotSize;
                    PlaceItem(new(x,y), slot);
                    this.AddChild(slot);
                    return true;
                }
            }
        }
        return false;
    }

    public bool InsertItem(Vector2I truePos, ItemSlot itemslot){
        if(ItemFits(truePos, itemslot)){
            itemslot.GridPosition = truePos;
            itemslot.Position = truePos * InventoryManager.SlotSize;
            PlaceItem(truePos, itemslot);
            return true;
        }
        return false;
    }

    public bool ItemFits(Vector2I truePos, BaseItem item){
        for (int y = truePos.Y; y < truePos.Y + item.ItemSize.Y; y++)
        {   
            for (int x = truePos.X; x < truePos.X + item.ItemSize.X; x++)
            {   
                if(!Slots[x,y].IsEmpty){
                     return false;
                }
            }
        }
        return true;
    }

    public bool ItemFits(Vector2I truePos, ItemSlot itemslot){
        for (int y = truePos.Y; y < truePos.Y + itemslot.ItemSize.Y; y++)
        {   
            for (int x = truePos.X; x < truePos.X + itemslot.ItemSize.X; x++)
            {   
                if(!Slots[x,y].IsEmpty){
                     return false;
                }
            }
        }
        return true;
    }

    private void PlaceItem(Vector2I truePos, ItemSlot item){
        for (int y = truePos.Y; y < truePos.Y + item.ItemSize.Y; y++)
        {   
            for (int x = truePos.X; x < truePos.X + item.ItemSize.X; x++)
            {   
                Slots[x,y].ItemSlot = item;
            }
        }
    }
}