using System;
using System.Diagnostics;
using System.IO;
using Unity.Burst;
using Debug = UnityEngine.Debug;

namespace DatabaseLibrary
{
    [BurstCompile]
    public static class DatabaseLoadExtension
    {
        public static void Load(this Database database, string filePath)
        {
            var watch = Stopwatch.StartNew();
            using var reader = new BinaryReader(File.OpenRead(filePath));

            var lenght = reader.ReadDouble();
            var spaceSegmentsCount = reader.ReadInt32();
            var timeSegmentsCount = reader.ReadInt32();

            var counductivities = new double[spaceSegmentsCount];
            var measurings = new TimeSlice[timeSegmentsCount];

            for (int i = 0; i < spaceSegmentsCount; i++)
            {
                counductivities[i] = reader.ReadDouble();
            }

            for (int i = 0; i < timeSegmentsCount; i++)
            {
                var time = reader.ReadDouble();
                var temperatures = new double[spaceSegmentsCount];

                for (int j = 0; j < spaceSegmentsCount; j++)
                {
                    temperatures[j] = reader.ReadDouble();
                }

                measurings[i] = new TimeSlice(time, temperatures);
            }

            database.Lenght = lenght;
            database.Conductivities = counductivities;
            database.Measurings = measurings;

            if (!database.Validate(out string error))
                throw new ArgumentException(error);

            watch.Stop();
            Debug.Log($"Database loaded and validated in {watch.ElapsedMilliseconds} ms. Lenght: {database.Lenght} | Space segments: {database.Conductivities.Length} | Time segments: {database.Measurings.Length}");
        }
    }
}
