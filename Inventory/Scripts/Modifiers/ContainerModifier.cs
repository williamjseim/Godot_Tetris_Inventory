using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Godot;
using Newtonsoft.Json;

public class ContainerModifier : ItemModifier, ISaveAble, ICloneable{

    public ContainerModifier() : base(){
        // this.ContainerSize = containerSize;
        // grid = new ItemSlot[containerSize.X, containerSize.Y];
        // this.FilterWhiteList = filterWhitelist;
        // this.FilterBlackList = filterBlacklist;
    }

    [JsonIgnore]
    private Vector2I _containerSize;
    public Vector2I ContainerSize { get {return _containerSize;} set{
        _containerSize = value;
        Grid = new IStorable[value.X, value.Y];
    } }

    [JsonIgnore]
    public IStorable[,] _grid;
    [JsonIgnore]
    public IStorable[,] Grid { get { return _grid; } protected set { _grid = value; } } // 2d grid cannot be serialized and deserialized

    // public List<IStorable> SerializedGrid { get { return _grid.Cast<IStorable>().ToList(); } set{ _grid = value; } }
    
    [JsonIgnore]
    public String[] FilterWhiteList { get; set; } // if empty its disabled
    [JsonIgnore]
    public String[] FilterBlackList { get; set; } // if empty its disabled

    public override string ToString()
    {
        return $"grid {Grid} whitelist {FilterWhiteList} blacklist {FilterBlackList}";
    }

    public object Save()
    {
        return new ContainerSaveData(this);
    }

    public void Load(object obj)
    {
        if(obj is ContainerSaveData saveData){
            if(saveData.Itemdata != null){
                for (var i = 0; i < saveData.Itemdata.Count; i++)
                {
                    this.insertSaveData(saveData.Itemdata[i]);
                }
            }
        }
    }

    private void insertSaveData(ItemData.SaveData data){
        ItemData itemData = new();
        itemData.Load(data);
        for (var y = data.gridPosition.Y; y < data.gridPosition.Y + itemData.ItemHolder.ItemSize.Y; y++)
        {
            for (var x = data.gridPosition.X; x < data.gridPosition.X + itemData.ItemHolder.ItemSize.X; x++)
            {
                if(y == data.gridPosition.Y && x == data.gridPosition.X){
                    this.Grid[x, y] = itemData;
                    continue;
                }
                this.Grid[x, y] = new ItemFiller(itemData.GridPosition, itemData);
            }
        }
    }

    public object Clone()
    {
        return new ContainerModifier(){
            FilterBlackList = this.FilterBlackList,
            FilterWhiteList = this.FilterWhiteList,
            ContainerSize = this.ContainerSize,
        };
    }

    [Serializable]
    public class ContainerSaveData : ModifierSaveData{
        public ContainerSaveData() : base(){}
        public ContainerSaveData(ContainerModifier modifier) : base(modifier)
        {
            Itemdata = new();
            foreach(var item in modifier.Grid){
                if(item is ItemData data){
                    this.Itemdata.Add((ItemData.SaveData)data.Save());
                }
            }
            this.DataAmount = Itemdata.Count;
        }
        public List<ItemData.SaveData> Itemdata { get; set; }
        public int DataAmount { get; set; }
    }
}
