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
		for (var X = 0; X < _containerModifier.ContainerSize.X; X++)
		{
			if(_containerModifier.grid[X,0] != null){
				Container.AddChild(_containerModifier.grid[X,0].Itemslot);
				_containerModifier.grid[X,0].Itemslot.Container = Container;
			}
		}
    }

    public override void Close()
    {
		base.Close();
		for (var X = 0; X < _containerModifier.ContainerSize.X; X++)
		{
			if(_containerModifier.grid[X,0] != null){
				Container.RemoveChild(_containerModifier.grid[X,0].Itemslot);
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
			Container.Slots = value.grid;
			this.Size += (value.ContainerSize - Vector2I.One) * InventoryManager.SlotSize;
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
