using Godot;
using Newtonsoft.Json;
using System;

public class BaseItem
{

  public BaseItem(){}
	public string Name { get;  set;}
	public string Id { get;  set; } = BaseItem.Empty; // id for saving item to json like how minecraft does it so ParentFolder:ItemName
  [JsonIgnore]
	public virtual Texture2D ItemSprite { get;  set; } // item image
  public string SpritePath { set { ItemSprite = ResourceLoader.Load<Texture2D>(value); } }
	public virtual int StackSize { get;  set; } = 999; // max stack size
  public virtual Vector2I ItemSize { get;  set; } = new Vector2I(1,1);

  /// <summary>
  ///  Quick way to mark item id as empty and make sure the spelling is alway correct
  /// </summary>
  public static string Empty { get { return "Empty"; } }
  public override bool Equals(object obj)
  {
    if(obj is BaseItem item)
      return this.Id == item.Id;

    return false;
  }

  public override int GetHashCode()
  {
    return base.GetHashCode();
  }
}
