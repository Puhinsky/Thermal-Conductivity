using System.Linq;
using Unity.Burst;

namespace DatabaseLibrary
{
    public static class DatabaseValidateExtension
    {
        public static bool Validate(this Database database, out string error)
        {
            database.Measurings = database.Measurings.OrderBy(m => m.Time).ToArray();

            error = string.Empty;
            var isValid = true;

            isValid &= database.ValidateForBeginTime(out string beginTimeError);
            isValid &= database.ValidateForBeginTemperatures(out string beginTemperaturesError);
            isValid &= database.ValidateForEdgeTemperatures(out string edgeTemperaturesError);
            isValid &= database.ValidateForNegativeConductivities(out string negativeConductivitiesError);
            isValid &= database.ValidateForNegativeTimes(out string negativeTimesError);

            error += beginTimeError + "\n";
            error += beginTemperaturesError + "\n";
            error += edgeTemperaturesError + "\n";
            error += negativeConductivitiesError + "\n";
            error += negativeTimesError;

            return isValid;
        }

        private static bool ValidateForBeginTime(this Database database, out string error)
        {
            if (database.Measurings.First().Time != 0d)
            {
                error = $"Invalid begin time: expected {0}, observed {database.Measurings.First().Time}";

                return false;
            }
            else
            {
                error = string.Empty;

                return true;
            }
        }

        private static bool ValidateForBeginTemperatures(this Database database, out string error)
        {
            if (database.Measurings.First().Temperatures.Any(t => t < 0))
            {
                error = "Non measuring temperatures in zero time detected";

                return false;
            }
            else
            {
                error = string.Empty;

                return true;
            }
        }

        private static bool ValidateForEdgeTemperatures(this Database database, out string error)
        {
            if (database.Measurings.Any(x => x.Temperatures.First() < 0 || x.Temperatures.Last() < 0))
            {
                error = "Non measuring temperatures on edges detected";

                return false;
            }
            else
            {
                error = string.Empty;

                return true;
            }
        }

        private static bool ValidateForNegativeConductivities(this Database database, out string error)
        {
            if (database.Conductivities.Any(x => x < 0d))
            {
                error = "Negative conductivities detected";

                return false;
            }
            else
            {
                error = string.Empty;

                return true;
            }
        }

        private static bool ValidateForNegativeTimes(this Database database, out string error)
        {
            if (database.Measurings.Any(x => x.Time < 0d))
            {
                error = "Negative times detected";

                return false;
            }
            else
            {
                error = string.Empty;

                return true;
            }
        }
    }
}