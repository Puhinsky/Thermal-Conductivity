using QFSW.QC;
using System.IO;
using UnityEngine;

namespace DatabaseLibrary
{
    public class DatabaseWrapper : MonoBehaviour
    {
        [Command("deserialize-db")]
        public async void DeserializeDatabase(string filePath)
        {
            var database = new Database();
            await database.Import(filePath);
            database.Save(Path.ChangeExtension(filePath, "db"));
        }

        [Command("serialize-db")]
        public async void SerializeDatabase(string filePath)
        {
            var database = Database.Load(filePath);
            await database.Export(Path.ChangeExtension(filePath, "txt"));
        }
    }
}
