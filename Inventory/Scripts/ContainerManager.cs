using Godot;
using System;

public partial class ContainerManager : Panel, IUi, IDoubleClick
{
    protected Slot[,] _slots;
    public Slot[,] Slots { get {return _slots; } }
    [Export] protected Vector2I inventorySize;
    [Export] GridContainer grid;
    [Export] protected Node container;
    [Export] PackedScene slotScene;

    public override void _Ready()
    {
        base._Ready();
        _slots = new Slot[inventorySize.X, inventorySize.Y];
        for (int y = 0; y < inventorySize.Y; y++)
        {
            for (int x = 0; x < inventorySize.X; x++)
            {
                var slot = slotScene.Instantiate<Slot>();
                slot.GridPosition = new Vector2I(x,y);
                _slots[x,y] = slot;
                grid.AddChild(slot);
            }
        }
    }

    public void OpenUi()
    {
        throw new NotImplementedException();
    }

    public void CloseUi()
    {
        throw new NotImplementedException();
    }

    public void Setup(ContainerItem item){
        
    }

    public void DoubleClick()
    {
        throw new NotImplementedException();
    }
}
