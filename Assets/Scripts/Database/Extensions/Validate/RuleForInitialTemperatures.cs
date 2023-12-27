using System.Linq;

namespace DatabaseLibrary
{
    internal class RuleForInitialTemperatures : IValidateRule
    {
        public bool Validate(Database database, out string error)
        {
            if (database.Temperatures[0..database.SpaceSegmentsCount].Any(t => double.IsNegative(t)))
            {
                error = "Non measuring temperatures in initial time detected";

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
