using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class ContainerWindow : BaseWindow
{
	[Export] public SlotContainer Container { get; private set; }
	public override void _Ready()
	{
		base._Ready();
	}

    public override void SetupModifiers(List<ItemModifier> modifiers)
    {
		foreach (var modifier in modifiers)
		{
			if(modifier is ContainerModifier containerModifier){
				this.ContainerModifier = containerModifier;
			}
		}
		for (var y = 0; y < _containerModifier.ContainerSize.Y; y++)
		{
			for (var x = 0; x < _containerModifier.ContainerSize.X; x++)
			{
				if(this.Container.Slots[x, y] is ItemData data){
					ItemSlot itemslot = InventoryManager.ItemslotScene.Instantiate<ItemSlot>();
                    itemslot.GridPosition = new(x, y);
                    itemslot.Container = Container;
                    itemslot.ItemHolder = data.ItemHolder;
                    itemslot.Size = data.ItemHolder.Item.ItemSize * InventoryManager.slotSize;
                    itemslot.Position = Container.GetSlotPosition(new Vector2I(x,y));
					itemslot.Rotated = data.ItemHolder.Rotated;
					this.Container.Slots[x, y].Data.Itemslot = itemslot;
					Container.AddChild(itemslot);
				} 				
			}
		}
    }

    public override void Close()
    {
		base.Close();
		for (var y = 0; y < _containerModifier.ContainerSize.Y; y++)
		{
			for (var X = 0; X < _containerModifier.ContainerSize.X; X++)
			{
				if(this.Container.Slots[X, y] is ItemData data){
					data.Itemslot?.QueueFree();
					data.Itemslot = null;
				} 				
			}
		}
		this.QueueFree();
    }

    private ContainerModifier _containerModifier;
	public ContainerModifier ContainerModifier
	{
		get { return _containerModifier; }
		set {
			_containerModifier = value;
			Container.ContainerSize = value.ContainerSize;
			Container.Slots = value.Grid;
			this.Size += (value.ContainerSize - Vector2I.One) * InventoryManager.slotSize;
		}
	}

	public bool whitelisted(string itemTypeName){
		if(_containerModifier.FilterWhiteList != null && _containerModifier.FilterWhiteList.Length > 0){
			return this._containerModifier.FilterBlackList.Contains(itemTypeName);
		}
		return true;
	}
	public bool BlackListed(string itemTypeName){
		if(_containerModifier.FilterBlackList == null){
			return false;
		}
		return this._containerModifier.FilterBlackList.Contains(itemTypeName);
	}
}
