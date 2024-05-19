using System.Collections.Generic;
using System.IO;
using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public static class SaveLoad{
    static string savefile = @"Inventory\SaveFile.json";

    public static void Save<T>(List<T> data){
        JsonSerializer serializer = new JsonSerializer();
        serializer.TypeNameHandling = TypeNameHandling.Objects;
        serializer.Formatting = Newtonsoft.Json.Formatting.Indented;
        serializer.MaxDepth = null;

        using(StreamWriter sw = new StreamWriter(savefile))
        using(JsonWriter writer = new JsonTextWriter(sw))
        {
            serializer.Serialize(writer, data);
        }
    }

    public static List<T> Load<T>(){
        JsonSerializer serializer = new JsonSerializer();
        serializer.TypeNameHandling = TypeNameHandling.Objects;
        serializer.Formatting = Newtonsoft.Json.Formatting.Indented;
        serializer.MaxDepth = null;

        List<T> itemHolders;
        using(StreamReader sr = new StreamReader(savefile))
        using(JsonReader reader = new JsonTextReader(sr)){
            itemHolders = serializer.Deserialize<List<T>>(reader);
        }
        return itemHolders;
    }
}