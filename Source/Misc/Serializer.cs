using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

public static class Serializer
{
    private static JsonSerializerSettings DefaultSettings => new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.None };

    private static JsonSerializerSettings IndentedSettings => new JsonSerializerSettings() 
    { 
        TypeNameHandling = TypeNameHandling.None,
        Formatting = Formatting.Indented
    };

    public static byte[] ConvertObjectToBytes(object toConvert)
    {
        JsonSerializer serializer = JsonSerializer.Create(DefaultSettings);
        MemoryStream memoryStream = new MemoryStream();

        using (BsonWriter writer = new BsonWriter(memoryStream)) 
        { 
            serializer.Serialize(writer, toConvert); 
        }

        return GZip.Compress(memoryStream.ToArray());
    }

    public static T ConvertBytesToObject<T>(byte[] bytes)
    {
        bytes = GZip.Decompress(bytes);

        JsonSerializer serializer = JsonSerializer.Create(DefaultSettings);
        MemoryStream memoryStream = new MemoryStream(bytes);

        using (BsonReader reader = new BsonReader(memoryStream)) 
        { 
            return serializer.Deserialize<T>(reader); 
        }
    }

    public static void SerializeToFile(string path, object serializable) { File.WriteAllText(path, JsonConvert.SerializeObject(serializable, IndentedSettings)); }

    public static T SerializeFromFile<T>(string path) { return JsonConvert.DeserializeObject<T>(File.ReadAllText(path), DefaultSettings); }
}