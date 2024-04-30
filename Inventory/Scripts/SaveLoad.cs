using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public static class SaveLoad{
    static string savefile = @"F:\GitHub\Godot_Tetris_Inventory\Inventory\SaveFile.json";

    public static void Save(List<ItemData> data){
        JsonSerializer serializer = new JsonSerializer();
        serializer.TypeNameHandling = TypeNameHandling.Objects;
        serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        serializer.Formatting = Newtonsoft.Json.Formatting.Indented;

        using(StreamWriter sw = new StreamWriter(savefile))
        using(JsonWriter writer = new JsonTextWriter(sw))
        {
            serializer.Serialize(writer, data);
        }
    }

    public static List<ItemData> Load(){
        JsonSerializer serializer = new JsonSerializer();
        serializer.TypeNameHandling = TypeNameHandling.Objects;
        serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        serializer.Formatting = Newtonsoft.Json.Formatting.Indented;

        List<ItemData> itemHolders;
        using(StreamReader sr = new StreamReader(@"F:\GitHub\Godot_Tetris_Inventory\Inventory\SaveFile.json"))
        using(JsonReader reader = new JsonTextReader(sr)){
            itemHolders = serializer.Deserialize<List<ItemData>>(reader);
        }
        return itemHolders;
    }
}