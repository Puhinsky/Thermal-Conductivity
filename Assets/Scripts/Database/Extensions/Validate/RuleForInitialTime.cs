using System.Linq;

namespace DatabaseLibrary
{
    internal class RuleForInitialTime : IValidateRule
    {
        public bool Validate(Database database, out string error)
        {
            if (database.Times.First() != 0d)
            {
                error = $"Invalid initial time: expected {0}, observed {database.Times.First()}";

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
