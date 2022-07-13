using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class XmlSaver<T> : FileSaver<T> where T : IDataStore
{
    // 

    public XmlSaver(string filename)
        : base(filename)
    {
    }

    public override void Save(T data)
    {
        Serialize(data, MFilename);
    }


    private static void Serialize(object item, string path)
    {
        XmlSerializer serializer = new XmlSerializer(item.GetType());
        StreamWriter writer = new StreamWriter(path);
        serializer.Serialize(writer.BaseStream, item);
        writer.Close();
    }


    public override bool Load(out T data)
    {
        if (!File.Exists(MFilename))
        {
            data = default(T);
            return false;
        }


        using var reader = GetReadStream();
        data = JsonUtility.FromJson<T>(reader.ReadToEnd());


        return true;
    }

    public override void Delete()
    {
        File.Delete(MFilename);
    }

    private JObject Json_To_Csv(string Json)
    {
        return JObject.Parse(Json);
    }
}