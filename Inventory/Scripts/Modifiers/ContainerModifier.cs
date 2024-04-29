using System;
using System.Text.Json.Serialization;
using Godot;

public class ContainerModifier : ItemModifier{

    public ContainerModifier() : base(){
        // this.ContainerSize = containerSize;
        // grid = new ItemSlot[containerSize.X, containerSize.Y];
        // this.FilterWhiteList = filterWhitelist;
        // this.FilterBlackList = filterBlacklist;
    }

    private Vector2I _containerSize;
    public Vector2I ContainerSize { get {return _containerSize;} set{
        _containerSize = value;
        grid = new GridFiller[value.X, value.Y];
    } }
    [JsonIgnore]
    public GridFiller[,] grid { get; protected set; }
    public String[] FilterWhiteList { get; set; } // if empty its disabled
    public String[] FilterBlackList { get; set; } // if empty its disabled

    public override string ToString()
    {
        return $"grid {grid} whitelist {FilterWhiteList} blacklist {FilterBlackList}";
    }

}