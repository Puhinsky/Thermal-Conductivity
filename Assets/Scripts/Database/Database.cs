using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Debug = UnityEngine.Debug;

namespace DatabaseLibrary
{
    [Serializable]
    public class Database
    {
        public double Lenght { get; set; }
        public double[] Conductivities { get; set; }
        public double[] Times { get; set; }
        public double[] Temperatures { get; set; }

        public int SpaceSegmentsCount => Conductivities.Length;
        public int TimeSegmentsCount => Times.Length;

        public Database()
        {
            Conductivities = Array.Empty<double>();
            Times = Array.Empty<double>();
            Temperatures = Array.Empty<double>();
        }

        public void Save(string filePath)
        {
            var watch = Stopwatch.StartNew();
            using var writer = File.Create(filePath);
            var formatter = new BinaryFormatter();
            formatter.Serialize(writer, this);
            watch.Stop();
            Debug.Log($"Database ({writer.Length} bytes) saved in {watch.ElapsedMilliseconds} ms: {filePath}");
        }

        public static Database Load(string filePath)
        {
            var watch = Stopwatch.StartNew();
            using var reader = File.OpenRead(filePath);
            var formatter = new BinaryFormatter();
            var database = formatter.Deserialize(reader) as Database;

            if (!database.Validate(out string error))
                throw new ArgumentException(error);

            watch.Stop();
            Debug.Log($"Database loaded and validated in {watch.ElapsedMilliseconds} ms. Lenght: {database.Lenght} | Space segments: {database.Conductivities.Length} | Time segments: {database.Times.Length}");

            return database;
        }
    }
}