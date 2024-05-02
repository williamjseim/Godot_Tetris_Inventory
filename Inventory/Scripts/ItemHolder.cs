using Godot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public struct ItemHolder : ISaveAble
{
	public ItemHolder(){
		this._item = null;
		this._amount = 0;
		this._staticModifiers = new();
		this.AddedModifiers = new ItemModifier[0];
	}
	Guid guid = Guid.NewGuid();
	
	public ItemHolder(BaseItem item, int amount)
	{
		this._item = item;
		_staticModifiers = new();
		if(item.Modifiers != null){
			foreach (var modifier in item.Modifiers)
			{
				if(modifier is ICloneable clone){
					_staticModifiers.Add((ItemModifier)clone.Clone());
				}
				else{
					_staticModifiers.Add(modifier);
				}
			}
		}
		else{
		_staticModifiers = new();
		}
		this._amount = amount;
		this.AddedModifiers = new ItemModifier[0];
	}

	public ItemHolder(SaveData saveData){
		this._item = ItemDatabase.Instance.GetItem(saveData.id);
		this._amount = saveData.amount;
		_staticModifiers = new();
		if(_item.Modifiers != null){
			foreach (var modifier in _item.Modifiers)
			{
				if(modifier is ICloneable clone){
					_staticModifiers.Add((ItemModifier)clone.Clone());
					GD.Print("modifier cloned itemholder");
				}
				else{
					_staticModifiers.Add(modifier);
				}
			}
		}
		else{
		_staticModifiers = new();
		}
		this.name = saveData.name;
		this.AddedModifiers = new ItemModifier[0];
		this.Rotated = saveData.Rotated;
		GD.Print(saveData.modifierStates.Count, " modifier states itemholder");
		for (var i = 0; i < saveData.modifierStates.Count; i++)
		{
			if(saveData.modifierStates[i] is ContainerModifier.ContainerSaveData modifier){
				GD.Print(modifier.Itemdata, " modifier data");
			}
			if(this._staticModifiers[i] is ISaveAble saveAble){
				saveAble.Load(saveData.modifierStates[i]);
			}	
		}
	}

	[JsonIgnore]
	private BaseItem _item;
	[JsonIgnore]
	public BaseItem Item { get{ return _item; } set{ this._item = value;} } // resources shouldnt be serialized and therefore not saved to a json file
	
	[JsonIgnore] public Texture2D Texture { get{ return this.Item == null ? null : this.Item.ItemTexture; } }

	public string Id { get { return this.Item == null ? BaseItem.Empty : Item.Id; } set {this._item = ItemDatabase.Instance.GetItem(value); GD.Print(Item, " item test"); this.StaticModifiers = _item.Modifiers; }} // -1 means the slot is empty

	public int _amount;
	public int Amount { get {return _amount; } set { _amount = value; } } //amount of items in slot

	/// <summary>
	/// Usefull if you want a item to have a custom name and dont wanna override the name the item resource has
	/// </summary>
	private string name = string.Empty;
	public string Name
	{
		get { return this.name == string.Empty ? this.Item.Name : this.name; }
		private set { name = value; }
	}
	public bool Rotated = false;
	
	[JsonIgnore]
	public Vector2I ItemSize { get { return Rotated ? new Vector2I(Item.ItemSize.Y, Item.ItemSize.X) : Item.ItemSize; } }

	private List<ItemModifier> _staticModifiers;
	public List<ItemModifier> StaticModifiers { get { return _staticModifiers; } set { _staticModifiers = value; } }
	public ItemModifier[] AddedModifiers;

	/// <summary>
	/// Use this to check if itemholders are equal
	/// </summary>
	/// <param name="obj"></param>
	/// <returns>True if itemholders are equal</returns>
	public override bool Equals(object obj)
	{
		if(obj is ItemHolder itemHolder && itemHolder.Id == this.Id && this.name == itemHolder.name && itemHolder.StaticModifiers == this.StaticModifiers && this.AddedModifiers == itemHolder.AddedModifiers)
		return true;

		return false;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public bool TryGetModifier<T>(out T modifier) where T : ItemModifier {
		if(this.StaticModifiers == null){
		modifier = null;
		return false;
		}
		modifier = (T)StaticModifiers.Where(i=>i is T).FirstOrDefault();
		var test = StaticModifiers.Where(i=>i is ContainerModifier).First();
		return modifier != null;
	}

	public override string ToString()
	{
		return $"Item name {Item.Name} Item id ${Item.Id} amount of items ${Amount}";
	}

    public object Save()
    {
        return new SaveData(this);
    }

    public void Load(object obj)
    {
		if(obj is SaveData data){
			this.Id = data.id;
			this._amount = data.amount;
			this.name = data.name;
			this.Rotated = data.Rotated;
			// for (var i = 0; i < data.modifierStates.Count; i++)
			// {
			// 	if(this.StaticModifiers[i] is ISaveAble saveAble){
			// 		GD.Print("new modifier itemholder");
			// 		saveAble.Load(data.modifierStates[i]);
			// 	}
			// }
		}
    }

	public static ItemHolder Load(SaveData obj){
		if(obj is SaveData data){
			ItemHolder itemHolder= new ItemHolder(){
				Id = data.id,
				_amount = data.amount,
				name = data.name,
				Rotated = data.Rotated,

			};
			for (var i = 0; i < data.modifierStates.Count; i++)
			{
				if(itemHolder.StaticModifiers[i] is ISaveAble saveAble){
					GD.Print("new modifier itemholder");
					saveAble.Load(saveAble);
					GD.Print(((ContainerModifier.ContainerSaveData)data.modifierStates[i]).Itemdata[0].itemholderSaveData.id);
				}
			}
		return itemHolder;
		}
		return new ItemHolder();
	}

    public static ItemHolder Empty => new ItemHolder();

	[Serializable]
	public class SaveData{
		public List<ItemModifier.ModifierSaveData> modifierStates = new();
		public string id;
		public int amount;
		public string name;
		public bool Rotated;
		public SaveData(){}
		public SaveData(ItemHolder itemHolder){
			this.id = itemHolder.Id;
			this.amount = itemHolder.Amount;
			this.name = itemHolder.Name;
			this.Rotated = itemHolder.Rotated;
			if(itemHolder.StaticModifiers != null)
			foreach (var item in itemHolder.StaticModifiers)
			{
				if(item is ISaveAble saveable){
					modifierStates.Add((ItemModifier.ModifierSaveData)saveable.Save());
				}
				else{
					modifierStates.Add(null);
				}
			}
		}
	}
}