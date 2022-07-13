using System;
using System.IO;
using UnityEngine;


public class JsonSaver<T> : FileSaver<T> where T : IDataStore
{
    public JsonSaver(string filename)
        : base(filename)
    {
        MFilename = $"{Application.persistentDataPath}/{filename}.json";
    }

    public override void Save(T data)
    {
        var json = JsonUtility.ToJson(data, true);
        
        using var writer = GetWriteStream();
        writer.Write(json);
    }

    public override bool Load(out T data)
    {
        if (!File.Exists(MFilename))
        {
            data = default(T);
            return false;
        }

        using var reader = GetReadStream();
        try
        {
            data = JsonUtility.FromJson<T>(reader.ReadToEnd());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            data = default(T);
            return false;
        }
        return true;
    }

    public override void Delete()
    {
        File.Delete(MFilename);
    }
}