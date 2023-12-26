using System.IO;
using UnityEngine;

namespace DatabaseLibrary
{
    public static class DatabaseSaveExtension
    {
        public static void Save(this Database database, string filePath)
        {
            using var writer = new BinaryWriter(File.Create(filePath));

            writer.Write(database.Lenght);
            writer.Write(database.Conductivities.Length);
            writer.Write(database.Measurings.Length);

            foreach (var conductivity in database.Conductivities)
            {
                writer.Write(conductivity);
            }

            foreach (var measuring in database.Measurings)
            {
                writer.Write(measuring.Time);

                foreach (var temperature in measuring.Temperatures)
                {
                    writer.Write(temperature);
                }
            }

            Debug.Log($"Database ({writer.BaseStream.Length} bytes) saved: {filePath}");
        }
    }
}