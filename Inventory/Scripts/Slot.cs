using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public partial class Slot : Panel
{
    [Export] protected Label amountLabel;
	public Vector2I GridPosition { get; set; }

	protected ItemSlot _itemSlot = null;
	/// <summary>
	/// use InsertItem to replace the itemholder
	/// </summary>
	public ItemSlot ItemSlot { get { return this._itemSlot; } set{
		this._itemSlot = value;
		if(value is null){

		}
	}}
	
	protected StyleBoxFlat stylebox = new(){BgColor = Colors.Transparent, BorderWidthBottom = 1, BorderWidthLeft = 1, BorderWidthRight = 1, BorderWidthTop = 1};
	public virtual bool IsEmpty {
		get{
			return ItemSlot is null;
		}
	}
	
	public static event Action<Vector2I> MouseEnter;
	public static event Action<Vector2I> MouseExit;

	public override void _Ready()
	{
		AssignEvents();
		this.AddThemeStyleboxOverride("panel", stylebox);
	}

	protected virtual void AssignEvents(){
		this.GuiInput += this.Input;
		this.MouseEntered += ()=>{ MouseEnter?.Invoke(this.GridPosition); };
		this.MouseExited += ()=>{ MouseExit?.Invoke(this.GridPosition); };
	}

	//communicates to the InventoryManager that a slot was clicked
	public virtual void Input(InputEvent input){
		if(input is InputEventMouseButton mouse && mouse.IsPressed()){}
	}

	public void HighLight(){
		this.stylebox.BgColor = Colors.Green;
	}
	public void DeHighLight(){
		this.stylebox.BgColor = Colors.Transparent;
	}
}