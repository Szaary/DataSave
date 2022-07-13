using Sirenix.Utilities;
using Steamworks;
using UnityEngine;

public class SteamSaver<T> : FileSaver<T> where T : IDataStore
{
    public SteamSaver(string filename) : base(filename)
    {
    }

    public override void Save(T data)
    {
        if (!SteamClient.IsValid)
        {
            Debug.LogError("Lost connection to steam");
        }

        if (!SteamRemoteStorage.IsCloudEnabled)
        {
            Debug.LogError("Cloud is not enabled for account");
        }

        var json = JsonUtility.ToJson(data, true);
        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        SteamRemoteStorage.FileWrite(BaseFileName, bytes);
    }

    public override bool Load(out T data)
    {
        if (!SteamRemoteStorage.FileExists(BaseFileName))
        {
            data = default(T);
            return false;
        }


        var bytes = SteamRemoteStorage.FileRead(BaseFileName);
        if (bytes == null)
        {
            data = default(T);
            return false;
        }

        var strings = System.Text.Encoding.UTF8.GetString(bytes);
        if (strings.IsNullOrWhitespace())
        {
            data = default(T);
            return false;
        }

        data = JsonUtility.FromJson<T>(strings);
        return true;
    }

    public override void Delete()
    {
        if (SteamRemoteStorage.FileExists(BaseFileName))
        {
            SteamRemoteStorage.FileDelete(BaseFileName);
        }
    }
}