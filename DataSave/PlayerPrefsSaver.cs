using Sirenix.Utilities;
using UnityEngine;

public class PlayerPrefsSaver<T> : FileSaver<T> where T : IDataStore
{
    public PlayerPrefsSaver(string filename) : base(filename)
    {
        MFilename = filename;
    }

    public override void Save(T data)
    {
        var json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(MFilename, json);
        PlayerPrefs.Save();
    }

    public override bool Load(out T data)
    {
        data = default(T);

        if (!PlayerPrefs.HasKey(MFilename))
        {
            return false;
        }
        
        var stringData = PlayerPrefs.GetString(MFilename);
        if (stringData.IsNullOrWhitespace())
        {
            return false;
        }

        data = JsonUtility.FromJson<T>(stringData);
        return true;
    }

    public override void Delete()
    {
        PlayerPrefs.DeleteKey(MFilename);
    }
}