using Godot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public struct ItemHolder
{
  public ItemHolder(){
    this._item = null;
    this._amount = 0;
    this._staticModifiers = new();
    this.AddedModifiers = new ItemModifier[0];
  }

  public ItemHolder(BaseItem item, int amount)
  {
    this._item = item;
    _staticModifiers = new();
    if(item.Modifiers != null){
      
      _staticModifiers = item.Modifiers;
    }
    else{
      _staticModifiers = null;
    }
    this._amount = amount;
    this.AddedModifiers = new ItemModifier[0];
  }

  [JsonIgnore]
  private BaseItem _item;
  
	[JsonIgnore]
	public BaseItem Item { get{ return _item; } set{
    this._item = value;
  } } // resources cant be serialized and therefore not saved to a json file
  
  [JsonIgnore] public Texture2D Texture { get{ return this.Item == null ? null : this.Item.ItemTexture; } }

	public string Id { get { return this.Item == null ? BaseItem.Empty : Item.Id; } set {this.Item = ItemDatabase.Instance.GetItem(value); }} // -1 means the slot is empty

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
    GD.Print(test);
    return modifier != null;
  }

  public override string ToString()
  {
    return $"Item name {Item.Name} Item id ${Item.Id} amount of items ${Amount}";
  }

    public static ItemHolder Empty => new ItemHolder();

}