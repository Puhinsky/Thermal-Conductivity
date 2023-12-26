using DatabaseLibrary;
using QFSW.QC;
using System.IO;
using UnityEngine;

public class DatabaseParser : MonoBehaviour
{
    [Command("deserialize-db")]
    public async void DeserializeDatabase(string filePath)
    {
        var database = new Database();
        await database.Import(filePath);
        database.Save(Path.ChangeExtension(filePath, "db"));
    }

    [Command("load-db")]
    public void LoadDatabase(string filePath)
    {
        var database = new Database();
        database.Load(filePath);
    }

    [Command("serialize-db")]
    public async void SerializeDatabase(string filePath)
    {
        var database = new Database();
        database.Load(filePath);
        await database.Export(Path.ChangeExtension(filePath, "txt"));
    }
}
