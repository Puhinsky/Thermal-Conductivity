using System.Collections.Generic;
using System.Linq;

namespace DatabaseLibrary
{
    public static class DatabaseValidateExtension
    {
        private static readonly IReadOnlyList<IValidateRule> _validateRules = new List<IValidateRule>()
        {
            new RuleForInitialTime(),
            new RuleForInitialTemperatures(),
            new RuleForNegativeTimes(),
            new RuleForNegativeConductivities(),
            new RuleForEdgeTemperatures()
        };

        public static bool Validate(this Database database, out string error)
        {
            var isValid = true;
            error = string.Empty;

            foreach (var rule in _validateRules)
            {
                isValid &= rule.Validate(database, out string ruleError);
                error += ruleError + "\n";
            }

            return isValid;
        }
    }
}