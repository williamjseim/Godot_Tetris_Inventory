using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

public class ItemData : IStorable, ISaveAble{
    public ItemData(){}
    public ItemData(Vector2I gridpos, ItemSlot itemslot)
    {
        this._itemslot = itemslot;
        this._itemholder = itemslot.ItemHolder; 
        this.GridPosition = gridpos;
    }

    private Vector2I _gridPos;
    public Vector2I GridPosition
    {
        get { return _gridPos; }
        set { _gridPos = value; }
    }

    private ItemHolder _itemholder;
    public ItemHolder ItemHolder { get { return _itemholder; } set { _itemholder = value; } }

    [JsonIgnore]
    private ItemSlot _itemslot;
    [JsonIgnore]
    public ItemSlot Itemslot { get{return _itemslot; } set { _itemslot = value;} }
    
    [JsonIgnore]
    public ItemData Data { get => this; set {} }

    public int CombineItems(ItemHolder item){
        return Data.Itemslot.CombineItems(item);
    }

    public object Save()
    {
        return new SaveData(this);
    }

    public void Load(object obj)
    {
        if(obj is SaveData saveData){

            this.ItemHolder = new(){Id = saveData.id, Amount = saveData.amount};
            this.GridPosition = saveData.gridPos;
        }
    }


    public class SaveData{
        public List<object> modifierStates = new();
        public string id;
        public int amount;
        public string name;
        public bool Rotated;
        public Vector2I gridPos;
        public SaveData(ItemData itemSlot){
            this.id = itemSlot.ItemHolder.Id;
            this.amount = itemSlot.ItemHolder.Amount;
            this.name = itemSlot.ItemHolder.Name;
            this.gridPos = itemSlot.GridPosition;
            this.Rotated = itemSlot.ItemHolder.Rotated;
            if(itemSlot.ItemHolder.StaticModifiers != null)
            foreach (var item in itemSlot.ItemHolder.StaticModifiers)
            {
                if(item is ISaveAble saveable){
                    modifierStates.Add(saveable.Save());
                }    
            }
        }
    }
}