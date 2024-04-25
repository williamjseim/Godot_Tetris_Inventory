using Godot;
using System;
using System.Linq;

public partial class ContainerWindow : Panel
{
	public override void _Ready()
	{

	}
	[Export] public SlotContainer Container { get; private set; }

	private ContainerModifier _modifier;
	public ContainerModifier Modifier
	{
		get { return _modifier; }
		set {
			_modifier = value;
			Container.Slots = value.grid;
			Container.ContainerSize = value.ContainerSize;
			this.Size += (value.ContainerSize - Vector2I.One) * InventoryManager.SlotSize;
		}
	}
	

}
