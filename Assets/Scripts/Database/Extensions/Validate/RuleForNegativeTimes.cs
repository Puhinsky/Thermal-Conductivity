using System.Linq;

namespace DatabaseLibrary
{
    internal class RuleForNegativeTimes : IValidateRule
    {
        public bool Validate(Database database, out string error)
        {
            if (database.Times.Any(t => double.IsNegative(t)))
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
