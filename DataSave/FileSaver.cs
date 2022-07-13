using System.IO;
using UnityEngine;

public abstract class FileSaver<T> : IDataSaver<T> where T : IDataStore
{
    protected string MFilename;
    protected readonly string BaseFileName;
    protected FileSaver(string filename)
    {
        BaseFileName = filename;
        MFilename = $"{Application.persistentDataPath}/{filename}";
    }

    public abstract void Save(T data);

    public abstract bool Load(out T data);

    public abstract void Delete();

    protected virtual StreamWriter GetWriteStream()
    {
        return new StreamWriter(new FileStream(MFilename, FileMode.Create));
    }

    protected virtual StreamReader GetReadStream()
    {
        return new StreamReader(new FileStream(MFilename, FileMode.Open));
    }
}