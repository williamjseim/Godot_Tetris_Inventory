using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public class Slot
{
	public Slot(Vector2I gridPos)
	{
		this.GridPosition = gridPos;
	}
	public Vector2I GridPosition { get; protected set; }
#nullable enable
	protected ItemSlot? _itemSlot;
	public ItemSlot? ItemSlot { get { return this._itemSlot; } set{ this._itemSlot = value; } }
#nullable disable
	public virtual bool IsEmpty {
		get{
			return ItemSlot == null;
		}
	}
}