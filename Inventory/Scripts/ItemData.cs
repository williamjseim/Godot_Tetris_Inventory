using System;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

public class ItemData : IStorable, ISaveAble{
    public ItemData(){}
    public ItemData(Vector2I gridpos, ItemSlot itemslot)
    {
        this._itemslot = itemslot;
        this._itemholder = itemslot.ItemHolder; 
        this._gridPos = gridpos;
    }

    private Vector2I _gridPos = new();
    public Vector2I GridPosition
    {
        get { return _gridPos; }
        set { _gridPos = value; }
    }

    private ItemHolder _itemholder = new();
    public ItemHolder ItemHolder { get { return _itemholder; } set { _itemholder = value; } }

    [JsonIgnore]
    private ItemSlot _itemslot = null;
    [JsonIgnore]
    public ItemSlot Itemslot { get{return _itemslot; } set { _itemslot = value;} }
    
    [JsonIgnore]
    public ItemData Data { get => this; set {} }

    public int CombineItems(ItemHolder item){
        return Data.Itemslot.CombineItems(item);
    }

    public virtual object Save()
    {
        return new SaveData(this);
    }

    public void Load(object obj)
    {
        if(obj is SaveData saveData){

            this.GridPosition = saveData.gridPosition;
            this.ItemHolder = new ItemHolder(saveData.itemholderSaveData);
            GD.Print(this.ItemHolder.Item, " new itemholder itemdata");
        }
    }

    [Serializable]
    public class SaveData{
        public Vector2I gridPosition;
        public ItemHolder.SaveData itemholderSaveData;
        public SaveData(){}
        public SaveData(ItemData itemSlot){
            this.gridPosition = itemSlot.GridPosition;
            this.itemholderSaveData = (ItemHolder.SaveData)itemSlot.ItemHolder.Save();
        }
    }
}