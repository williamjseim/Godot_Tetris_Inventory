using Godot;

public partial class ContainerWindow : BaseWindow
{
	[Export] public SlotContainer Container { get; private set; }
	public override void _Ready()
	{
		base._Ready();
	}

    public override void SetupModifiers(ItemModifier[] modifiers)
    {
		foreach (var modifier in modifiers)
		{
			if(modifier is ContainerModifier containerModifier){
				this.ContainerModifier = containerModifier;
			}
		}
    }

    public override void Close()
    {
		this.Itemslot.ItemHolder.TryGetModifier<ContainerModifier>(out ContainerModifier modifier);
		modifier = this.ContainerModifier;
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
	

}
