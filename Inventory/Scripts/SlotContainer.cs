using System;
using Godot;

public partial class SlotContainer : ContainerManager{

    public IStorable[,] Slots { get; set;}
    [Export] public Vector2I ContainerSize { get; set; } = Vector2I.One;
    public static event Action<SlotContainer> MouseEnteredContainer;
    public static event Action<SlotContainer> MouseLeftContainer;
    public static event Action<SlotContainer, InputEventMouseButton, ItemSlot> MousePressed;
    public static event Action<SlotContainer, InputEventMouse> MouseReleased;
    public static event Action<SlotContainer, InputEventMouseButton, ItemSlot> MouseDoubleClick;

    public Highlight HighlightPanel { get; protected set; }

    public override void _Ready()
    {
        base._Ready();
        CustomMinimumSize = ContainerSize*InventoryManager.slotSize;
        UpdateMinimumSize();
        MouseEntered += MouseEnter;
        MouseExited += MouseExit;
        this.Slots = new IStorable[ContainerSize.X, ContainerSize.Y];
        if(HighlightPanel == null){
            HighlightPanel = new();
        }
        this.AddChild(HighlightPanel);
        HighlightPanel.Visible = false;
    }
    protected virtual void MouseEnter(){
        MouseEnteredContainer?.Invoke(this);
    }

    protected virtual void MouseExit(){
        MouseLeftContainer?.Invoke(this);
        this.HighlightPanel.Visible = false;
    }

    public override void _Draw()
    {
        base._Draw();
        for (int y = 0; y < ContainerSize.Y; y++)
        {
            for (int x = 0; x < ContainerSize.X; x++)
            {
                var rect = new Rect2(new Vector2I(x,y) * InventoryManager.slotSize, Vector2.One * InventoryManager.slotSize);
                DrawRect(rect, Colors.White, false);
            }
        }
    }

    public Vector2I GetSlotIndex(Vector2 pos, Vector2I itemsize = new Vector2I()){
        return ((Vector2I)pos/InventoryManager.slotSize).Clamp(Vector2I.Zero, this.ContainerSize - itemsize);
    }

    public Vector2I GlobalToSlotIndex(Vector2 pos, Vector2I itemsize = new()){
        pos = (pos - this.GlobalPosition).Abs();
        return ((Vector2I)pos/InventoryManager.slotSize).Clamp(Vector2I.Zero, this.ContainerSize - itemsize);
    }

    public Vector2 GetSlotPosition(Vector2I index){
        return index*InventoryManager.slotSize;
    }

    public Vector2 GetGlobalSlotPosition(Vector2I index){
        return (index*InventoryManager.slotSize + this.GlobalPosition).Abs();
    }

    public Vector2 MouseToSlotPosition(Vector2 mousePos, Vector2I itemsize = new Vector2I()){
        return GetSlotPosition(GetSlotIndex(mousePos).Clamp(Vector2I.Zero, this.ContainerSize - itemsize));
    }

    public Vector2 MouseToGlobalSlotPosition(Vector2 localMousePos, Vector2I itemsize = new Vector2I()){
        return (GetSlotPosition(GetSlotIndex(localMousePos).Clamp(Vector2I.Zero, this.ContainerSize - itemsize)) + this.GlobalPosition).Abs();
    }

    public override void ContainerInput(InputEvent @event){
        if(@event is InputEventMouseButton button){
            Vector2I index = GetSlotIndex(button.Position);
            if(button.DoubleClick){
                if(Slots[index.X, index.Y] != null){
                    MouseDoubleClick?.Invoke(this, button, Slots[index.X, index.Y].Data.Itemslot);
                }
            }
            else if(button.ButtonMask == MouseButtonMask.Left && button.IsPressed()){
                if(this.Slots[index.X, index.Y] != null){
                    MousePressed?.Invoke(this, button, Slots[index.X, index.Y].Data.Itemslot);
                }
            }
            else if(button.ButtonIndex == MouseButton.Left && button.IsReleased()){
                MouseReleased?.Invoke(this, button);
                this.HighlightPanel.Visible = false;
            }
            else if(button.ButtonIndex == MouseButton.Right && button.IsPressed()){
                var slot = Slots[index.X, index.Y];
                if(slot != null){
                    GD.Print("Type", slot.GetType().Name," ", slot.Data.ItemHolder.Item.Name, " item name", slot?.Data.ItemHolder.StaticModifiers?.Count);
                }
            }
        }
        if(@event is InputEventMouseMotion motion){
            this.MouseMotionInvoke(motion);
        }
    }

    protected override void MouseMotionInvoke(InputEventMouseMotion motion)
    {
        base.MouseMotionInvoke(motion);
    }

    public virtual bool InsertItem(ItemHolder item){
        if(CheckForExistingItem(item, out ItemSlot slot)){
            int i = slot.CombineItems(item);
            if(i <= 0){
                return true;
            }
            item.Amount = i;
        }
        for (int y = 0; y <= ContainerSize.Y - item.Item.ItemSize.Y; y++)
        {
            for (int x = 0; x <= ContainerSize.X - item.Item.ItemSize.X; x++)
            {
                if(ItemFits(new Vector2I(x, y), item.Item)){
                    ItemSlot itemslot = InventoryManager.ItemslotScene.Instantiate<ItemSlot>();
                    itemslot.GridPosition = new(x, y);
                    itemslot.Container = this;
                    itemslot.ItemHolder = item;
                    itemslot.Size = item.Item.ItemSize * InventoryManager.slotSize;
                    itemslot.Position = this.GetSlotPosition(new Vector2I(x,y)); //new Vector2I(x, y) * InventoryManager.SlotSize;
                    PlaceItem(new(x,y), itemslot);
                    this.AddChild(itemslot);
                    return true;
                }
            }
        }
        return false;
    }

    public virtual bool InsertItem(ItemData.SaveData item){
        ItemData itemData = new();
        itemData.Load(item);
        for (var y = item.gridPosition.Y; y < itemData.ItemHolder.ItemSize.Y + item.gridPosition.Y; y++)
        {
            for (var x = item.gridPosition.X; x < itemData.ItemHolder.ItemSize.X + item.gridPosition.X; x++)
            {
                if( new Vector2I(x, y) == item.gridPosition){
                    ItemSlot itemslot = InventoryManager.ItemslotScene.Instantiate<ItemSlot>();
                    itemslot.GridPosition = new(x, y);
                    itemslot.Container = this;
                    itemslot.ItemHolder = itemData.ItemHolder;
                    itemslot.Size = itemData.ItemHolder.ItemSize * InventoryManager.slotSize;
                    itemslot.Position = this.GetSlotPosition(new Vector2I(x,y));
                    itemData.Itemslot = itemslot;
                    this.Slots[x,y] = itemData;
                    this.AddChild(itemslot);
                    continue;
                }
                Slots[x, y] = new ItemFiller(item.gridPosition, itemData);
            }
        }
        return true;
    }

    public bool CheckForExistingItem(ItemHolder item, out ItemSlot ExistingItem){
        for (int y = 0; y < ContainerSize.Y; y++)
        {
            for (int x = 0; x < ContainerSize.X; x++)
            {
                if(Slots[x, y] != null && Slots[x, y].Data.Itemslot.ItemHolder.Equals(item) && !Slots[x, y].Data.Itemslot.IsFull){
                    ExistingItem = Slots[x, y].Data.Itemslot;
                    return true;
                }
            }
        }
        ExistingItem = null;
        return false;
    }

    public virtual bool InsertItem(Vector2I truePos, ItemSlot itemslot){
        if(ItemFits(truePos, itemslot)){
            if(itemslot.Container != this){
                itemslot.Container.RemoveChild(itemslot);
                this.AddChild(itemslot);
            }
            itemslot.Container.RemoveItem(itemslot);
            itemslot.GridPosition = truePos;
            itemslot.Position = truePos * InventoryManager.slotSize;
            PlaceItem(truePos, itemslot);
            itemslot.Container = this;
            itemslot.ZIndex = 0;
            return true;
        }
        return false;
    }

    public virtual bool ItemFits(Vector2I truePos, BaseItem item){
        for (int y = truePos.Y; y < truePos.Y + item.ItemSize.Y; y++)
        {   
            for (int x = truePos.X; x < truePos.X + item.ItemSize.X; x++)
            {   
                if(Slots[x,y] != null){
                     return false;
                }
            }
        }
        return true;
    }

    public virtual bool ItemFits(Vector2I truePos, ItemSlot itemslot){
        for (int y = truePos.Y; y < truePos.Y + itemslot.ItemSize.Y; y++)
        {   
            for (int x = truePos.X; x < truePos.X + itemslot.ItemSize.X; x++)
            {
                if(Slots[x,y] != null && Slots[x,y].Data.Itemslot != itemslot){
                    return false;
                }
            }
        }
        return true;
    }

    private void PlaceItem(Vector2I truePos, ItemSlot itemslot){
        ItemData data = new ItemData(truePos, itemslot);
        for (int y = truePos.Y; y < truePos.Y + itemslot.ItemSize.Y; y++)
        {   
            for (int x = truePos.X; x < truePos.X + itemslot.ItemSize.X; x++)
            {   
                if(x == truePos.X && y == truePos.Y){
                    Slots[x,y] = data;
                }
                else{
                    ItemFiller filler = new ItemFiller(new(truePos.X, truePos.Y), data);
                    this.Slots[x,y] = filler;
                }
            }
        }
    }

    public void RemoveItem(ItemSlot itemSlot){
        Vector2I trueSize;
        if(itemSlot.JustRotated){
            trueSize = new Vector2I(itemSlot.ItemSize.Y, itemSlot.ItemSize.X);
        }
        else{
            trueSize = itemSlot.ItemSize;
        }
        for (int y = itemSlot.GridPosition.Y; y < itemSlot.GridPosition.Y + trueSize.Y ; y++)
        {
            for (int x = itemSlot.GridPosition.X; x < itemSlot.GridPosition.X + trueSize.X ; x++)
            {
                this.Slots[x,y] = null;
            }
            
        }
    }

    public void Highlight(ItemSlot itemslot){
        if(!this.HighlightPanel.Visible){
            this.HighlightPanel.Show();
        }
        Vector2I index = GlobalToSlotIndex(itemslot.TruePosition, itemslot.ItemSize);
        this.HighlightPanel.Position = GetSlotPosition(index);
        this.HighlightPanel.Size = itemslot.ItemSize * InventoryManager.slotSize;
        this.HighlightPanel.Fits = ItemFits(index, itemslot);
    }

    public void ResetItem(ItemSlot itemSlot){
        if(itemSlot.JustRotated){
            itemSlot.Rotated = !itemSlot.Rotated;
        }
        itemSlot.Position = GetSlotPosition(itemSlot.GridPosition);
    }
}