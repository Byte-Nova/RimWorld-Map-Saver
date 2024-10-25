using System.IO;
using Newtonsoft.Json;

public static class Serializer
{
    private static JsonSerializerSettings DefaultSettings => new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.None };

    private static JsonSerializerSettings IndentedSettings => new JsonSerializerSettings() 
    { 
        TypeNameHandling = TypeNameHandling.None,
        Formatting = Formatting.Indented
    };

    public static void SerializeToFile(string path, object serializable) { File.WriteAllText(path, JsonConvert.SerializeObject(serializable, IndentedSettings)); }

    public static T SerializeFromFile<T>(string path) { return JsonConvert.DeserializeObject<T>(File.ReadAllText(path), DefaultSettings); }
}