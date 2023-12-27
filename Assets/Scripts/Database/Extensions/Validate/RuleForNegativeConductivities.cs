using System.Linq;

namespace DatabaseLibrary
{
    internal class RuleForNegativeConductivities : IValidateRule
    {
        public bool Validate(Database database, out string error)
        {
            if (database.Conductivities.Any(c => double.IsNegative(c)))
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
    }
}
