using Godot;
using System;
using System.Linq;
using System.Text.Json.Serialization;

[Serializable]
public struct ItemHolder
{
  public ItemHolder(){
    this._item = null;
    this._amount = 0;
    this.StaticModifiers = new ItemModifier[0];
    this.AddedModifiers = new ItemModifier[0];
  }

  public ItemHolder(BaseItem item, int amount)
  {
    this._item = item;
    this.StaticModifiers = item.Modifiers;
    GD.Print(item.Modifiers, " modifiers");
    this._amount = amount;
    this.AddedModifiers = new ItemModifier[0];
  }

  private BaseItem _item;
  
	[JsonIgnore]
	public BaseItem Item { get{ return _item; } set{
    this._item = value;
  } } // resources cant be serialized and therefore not saved to a json file
  
  [JsonIgnore]
  public Texture2D Texture { get{ return this.Item == null ? null : this.Item.ItemTexture; } }

	public string Id { get { return this.Item == null ? BaseItem.Empty : Item.Id; }} // -1 means the slot is empty
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

  public ItemModifier[] StaticModifiers;
  public ItemModifier[] AddedModifiers;
  
  /// <summary>
  /// Use this to check if itemholders are equal
  /// </summary>
  /// <param name="obj"></param>
  /// <returns>True if itemholders are equal</returns>
  public override bool Equals(object obj)
  {
    if(obj is ItemHolder itemHolder && itemHolder.Id == this.Id && this.name == itemHolder.name)
      return true;

    return false;
  }

  public override int GetHashCode()
  {
      return base.GetHashCode();
  }

  public bool TryGetModifier<T>(out T modifier) where T : ItemModifier {
    modifier = (T)StaticModifiers.Where(i=>i is T).FirstOrDefault();
    var test = StaticModifiers.Where(i=>i is ContainerModifier).First();
    GD.Print(test);
    return modifier != null;
  }

    public static ItemHolder Empty => new ItemHolder();

}