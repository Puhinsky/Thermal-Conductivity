using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace DatabaseLibrary
{
    public static class DatabaseExportExtension
    {
        public static async Task Export(this Database database, string filePath)
        {
            var result = string.Empty;

            result = result.WriteGroup("GRID", database.ExportGridGroup());
            result = result.WriteGroup("TUBE", database.ExportTubeGroup());
            result = result.WriteGroup("TEMP", database.ExportTempGroup());

            await File.WriteAllTextAsync(filePath, result);

            Debug.Log($"Database exported: {filePath}");
        }

        private static string WriteGroup(this string destination, string groupTag, string groupValue)
        {
            return destination + $"{groupTag}\r\n{groupValue} /\r\n";
        }

        private static string ExportGridGroup(this Database database)
        {
            return $"{database.Lenght} {database.Conductivities.Length} {database.Measurings.Length}";
        }

        private static string ExportTubeGroup(this Database database)
        {
            return string.Join(" ",database.Conductivities.Select(x=>x.ToString(CultureInfo.InvariantCulture)));
        }

        private static string ExportTempGroup(this Database database)
        {
            return string.Join(" ", database.Measurings.Select(m => m.ToString()));
        }
    }
}
