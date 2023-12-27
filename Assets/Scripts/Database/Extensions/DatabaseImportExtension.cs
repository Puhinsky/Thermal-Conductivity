using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

namespace DatabaseLibrary
{
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
            database.SetConductivities(tube, spaceSegments);
            database.SetTimesAndTemperatures(temp, spaceSegments, timeSegments);

            if(!database.Validate(out string error))
                throw new ArgumentException(error);

            watch.Stop();
            Debug.Log($"Database imported and validated in {watch.ElapsedMilliseconds} ms. Lenght: {database.Lenght} | Space segments: {database.Conductivities.Length} | Time segments: {database.Times.Length}");
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

        private static void SetConductivities(this Database database, string groupValue, int spaceSegments)
        {
            if (string.IsNullOrEmpty(groupValue))
            {
                Debug.Log("Conductivities are not exist");

                database.Conductivities = new double[spaceSegments];
            }
            else
            {
                var splitted = groupValue.Split();

                if (splitted.Length != spaceSegments)
                    throw new ArgumentException($"Invalid conductivities count: expected {spaceSegments}, observed {splitted.Length}");

                database.Conductivities = splitted.Select(x => Convert.ToDouble(x, CultureInfo.InvariantCulture)).ToArray();
            }
        }

        private static void SetTimesAndTemperatures(this Database database, string groupValue, int spaceSegments, int timeSegments)
        {
            var rawValues = groupValue.Split().Select(x => Convert.ToDouble(x, CultureInfo.InvariantCulture)).ToArray();
            var expectedCount = timeSegments * (spaceSegments + 1);

            if (rawValues.Length != expectedCount)
                throw new ArgumentException($"Invalid measuring count: expected {expectedCount}, observed {rawValues.Length}");

            database.Times = new double[timeSegments];
            database.Temperatures = new double[spaceSegments * timeSegments];

            for (int time = 0; time < timeSegments; time++)
            {
                var firstIndexInRow = time * (spaceSegments + 1);
                var lastIndexInRow = firstIndexInRow + spaceSegments;
                database.Times[time] = rawValues[firstIndexInRow];
                Array.Copy(rawValues, firstIndexInRow + 1, database.Temperatures, time * spaceSegments, spaceSegments);
            }
        }
    }
}
