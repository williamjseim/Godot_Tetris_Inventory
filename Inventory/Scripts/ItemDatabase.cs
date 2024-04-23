using Godot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

public class ItemDatabase
{
    private static ItemDatabase _instance;
    public static ItemDatabase Instance { get { if(_instance == null){ _instance = new(); } return _instance; } }

    string baseItemFolder = "res://Inventory/ItemParent";//change this to what ever path matches your project

    public ItemDatabase()
    {
    }
    
    public Dictionary<string, BaseItem> Items = new();

    public BaseItem this[string key]{
        get { return this.Items[key]; }
    }
    
    public BaseItem GetItem(string id){
        if(id == BaseItem.Empty){
            return null;
        }
        if(this.Items.TryGetValue(id, out BaseItem item)){
            return item;
        }
        GD.PrintErr($"Item id does not exist {id}");
        return null;
    }

    public void OverrideItemId(BaseItem item, string id){
        //BaseItem newItem = new(id, item);
        // ResourceSaver.Save(newItem, item.ResourcePath);
    }

    public void LoadItems(){
        JsonSerializer serializer = new JsonSerializer();
        serializer.TypeNameHandling = TypeNameHandling.Objects;
        serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        serializer.Formatting = Newtonsoft.Json.Formatting.Indented;

        // using (FileStream fileStream = new FileStream(@"F:\GitHub\Godot_Tetris_Inventory\Inventory\ItemParent\Items.xml", FileMode.Open))
        // using(StreamWriter sw = new StreamWriter(@"F:\GitHub\Godot_Tetris_Inventory\Inventory\ItemParent\JsonItems.menace"))
        // using(JsonWriter writer = new JsonTextWriter(sw))
        // {
        //     serializer.Serialize(writer, items);
        // }

        BaseItem[] array;
        using(StreamReader sr = new StreamReader(@".\Inventory\ItemParent\JsonItems.menace"))
        using(JsonReader reader = new JsonTextReader(sr)){
            array = serializer.Deserialize<BaseItem[]>(reader);
        }

        
        string[] foldersplit = baseItemFolder.Split("/");
        string itemParent = foldersplit[foldersplit.Length-2];
        foreach (var item in array)
        {
            GD.Print(item.Id);
            if(item.Id != string.Empty && item.Id != BaseItem.Empty){
                try{
                    this.Items.Add(item.Id, item);
                    continue;
                }
                catch(Exception ex){ GD.PrintErr(ex); }
            }
            string itemId = $"{itemParent}:{item.Name}";
            if(item.Id == BaseItem.Empty || item.Id == ""){
                GD.PrintErr("Item without id and was not added to the dictionary");
            }
        }
    }
}
