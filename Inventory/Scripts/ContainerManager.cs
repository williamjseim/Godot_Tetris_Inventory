using Godot;
using System;

public partial class ContainerManager : Panel
{
    protected Slot[,] _slots;
    public Slot[,] Slots { get {return _slots; } }
    [Export] protected Vector2I inventorySize;
    [Export] GridContainer grid;
    [Export] protected MarginContainer container;
    [Export] PackedScene slotScene;

    public override void _Ready()
    {
        base._Ready();
        _slots = new Slot[inventorySize.X, inventorySize.Y];
        for (int y = 0; y < inventorySize.Y; y++)
        {
            for (int X = 0; X < inventorySize.X; X++)
            {
                GD.Print("sdasdsadw");
                var slot = slotScene.Instantiate<Slot>();
                _slots[X,y] = slot;
                grid.AddChild(slot);
            }
        }
    }


}
