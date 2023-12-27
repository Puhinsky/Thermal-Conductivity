using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

namespace DatabaseLibrary
{
    public static class DatabaseExportExtension
    {
        public static async Task Export(this Database database, string filePath)
        {
            var watch = Stopwatch.StartNew();
            var result = string.Empty;

            result = result.WriteGroup("GRID", database.ExportGridGroup());
            result = result.WriteGroup("TUBE", database.ExportTubeGroup());
            result = result.WriteGroup("TEMP", database.ExportTempGroup());

            await File.WriteAllTextAsync(filePath, result);

            watch.Stop();
            Debug.Log($"Database exported in {watch.ElapsedMilliseconds} ms: {filePath}");
        }

        private static string WriteGroup(this string destination, string groupTag, string groupValue)
        {
            return destination + $"{groupTag}\r\n{groupValue} /\r\n";
        }

        private static string ExportGridGroup(this Database database)
        {
            return $"{database.Lenght} {database.SpaceSegmentsCount} {database.TimeSegmentsCount}";
        }

        private static string ExportTubeGroup(this Database database)
        {
            return string.Join(" ", database.Conductivities.Select(x => x.ToString(CultureInfo.InvariantCulture)));
        }

        private static string ExportTempGroup(this Database database)
        {
            var result = new List<string>();

            for (int time = 0; time < database.TimeSegmentsCount; time++)
            {
                var firstIndexInRow = time * database.SpaceSegmentsCount;
                var lastIndexInRow = firstIndexInRow + database.SpaceSegmentsCount;

                result.Add(Convert.ToString(database.Times[time], CultureInfo.InvariantCulture));
                result.Add(string.Join(" ", database.Temperatures[firstIndexInRow..lastIndexInRow].Select(x => Convert.ToString(x, CultureInfo.InvariantCulture))));
            }

            return string.Join(" ", result);
        }
    }
}
