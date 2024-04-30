using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Newtonsoft.Json;

public class ContainerModifier : ItemModifier, ISaveAble{

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
        throw new NotImplementedException();
    }

    private void insertSaveData(ItemData data){
        for (var y = data.GridPosition.Y; y < data.GridPosition.Y + data.ItemHolder.ItemSize.Y; y++)
        {
            for (var x = data.GridPosition.X; x < data.GridPosition.X + data.ItemHolder.ItemSize.X; x++)
            {
                if(y == data.GridPosition.Y && x == data.GridPosition.X){
                    this.Grid[y, x] = data;
                    continue;
                }
                this.Grid[y, x] = new ItemFiller(data.GridPosition, data);
            }
        }
    }

    public class ContainerSaveData : ModifierSaveData{
        Vector2I containerSize;
        public List<object> Itemdata { get; private set; }
        public ContainerSaveData() : base(null) {}
        public ContainerSaveData(ContainerModifier modifier) : base(modifier)
        {
            Itemdata = new();
            this.containerSize = modifier.ContainerSize;
            foreach(var item in modifier.Grid){
                if(item is ItemData data){
                    this.Itemdata.Add(data.Save());
                }
            }
        }

        public override ItemModifier CreateInstance(){
            var modifier = new ContainerModifier(){
                Grid = new IStorable[containerSize.X, containerSize.Y],
            };
            foreach (var item in Itemdata)
            {
                modifier.insertSaveData((ItemData)item);
            }
            return modifier;
        }
    }
}
