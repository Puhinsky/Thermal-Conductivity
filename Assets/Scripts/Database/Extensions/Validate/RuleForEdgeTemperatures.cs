using System;

namespace DatabaseLibrary
{
    internal class RuleForEdgeTemperatures : IValidateRule
    {
        public bool Validate(Database database, out string error)
        {
            for (int time = 0; time < database.TimeSegmentsCount; time++)
            {
                var firstIndexInRow = time * database.SpaceSegmentsCount;
                var lastIndexInRow = firstIndexInRow + database.SpaceSegmentsCount - 1;

                if (double.IsNegative(database.Temperatures[firstIndexInRow]) || double.IsNegative(database.Temperatures[lastIndexInRow]))
                {
                    error = "Non measuring temperatures on edges detected";

                    return false;
                }
            }

            error = string.Empty;

            return true;
        }
    }
}
