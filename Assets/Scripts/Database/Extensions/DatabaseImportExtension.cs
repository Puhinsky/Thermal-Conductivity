using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.Burst;
using Debug = UnityEngine.Debug;

namespace DatabaseLibrary
{
    [BurstCompile]
    public static class DatabaseImportExtension
    {
        public static async Task Import(this Database database, string filePath)
        {
            var watch = Stopwatch.StartNew();
            var text = await File.ReadAllTextAsync(filePath);
            Debug.Log("File readed");

            text = RemoveComments(text);

            var grid = GetAndValidGrid(FindGroup(text, "GRID"));
            var tube = FindGroup(text, "TUBE");
            var temp = FindGroup(text, "TEMP");

            var spaceSegments = Convert.ToInt32(grid[1], CultureInfo.InvariantCulture);
            var timeSegments = Convert.ToInt32(grid[2], CultureInfo.InvariantCulture);

            database.Lenght = Convert.ToDouble(grid[0], CultureInfo.InvariantCulture);
            database.Conductivities = GetAndValidConductivities(tube, spaceSegments);
            database.Measurings = GetAndValidTimeSlices(temp, spaceSegments, timeSegments);

            if(!database.Validate(out string error))
                throw new ArgumentException(error);

            watch.Stop();
            Debug.Log($"Database imported and validated in {watch.ElapsedMilliseconds} ms. Lenght: {database.Lenght} | Space segments: {database.Conductivities.Length} | Time segments: {database.Measurings.Length}");
        }

        private static string RemoveComments(string text)
        {
            text = Regex.Replace(text, @"(--).*\n", "");
            text = text.Replace("\r\n", " ");

            return text;
        }

        private static string FindGroup(string text, string tag)
        {
            var pattern = @$"(?<={tag}\s)[-\s\d\.]*(?=\s\/)";
            var match = Regex.Match(text, pattern);

            if (!match.Success)
                Debug.Log($"Tag {tag} not founded while parsing");

            return match.Value;
        }

        private static string[] GetAndValidGrid(string groupValue)
        {
            var result = groupValue.Split();

            if (result.Length != 3)
                throw new ArgumentException($"Invalid grid arguments count expected: {3}, observed {result.Length}");

            return result;
        }

        private static double[] GetAndValidConductivities(string groupValue, int spaceSegments)
        {
            if (string.IsNullOrEmpty(groupValue))
            {
                Debug.Log("Conductivities are not exist");

                return new double[spaceSegments];
            }

            var splitted = groupValue.Split();

            if (splitted.Length != spaceSegments)
                throw new ArgumentException($"Invalid conductivities count: expected {spaceSegments}, observed {splitted.Length}");

            var result = splitted.Select(x => Convert.ToDouble(x, CultureInfo.InvariantCulture)).ToArray();

            if (result.Any(x => x < 0d))
                throw new ArgumentException($"Negative conductivity detected: {result.Where(x => x < 0d)}");

            return result;
        }

        private static TimeSlice[] GetAndValidTimeSlices(string groupValue, int spaceSegments, int timeSegments)
        {
            var splitted = groupValue.Split();
            var expectedCount = timeSegments * (spaceSegments + 1);

            if (splitted.Length != expectedCount)
                throw new ArgumentException($"Invalid temperatures count: expected {expectedCount}, observed {splitted.Length}");

            var result = new TimeSlice[timeSegments];

            for (int i = 0; i < timeSegments; i++)
            {
                var firstIndexInRow = i * (spaceSegments + 1);
                var lastIndexInRow = firstIndexInRow + spaceSegments;
                var time = Convert.ToDouble(splitted[firstIndexInRow], CultureInfo.InvariantCulture);
                var temperatures = splitted[(firstIndexInRow + 1)..(lastIndexInRow + 1)].Select(x => Convert.ToDouble(x, CultureInfo.InvariantCulture)).ToArray();

                result[i] = new TimeSlice(time, temperatures);
            }

            return result;
        }
    }
}
