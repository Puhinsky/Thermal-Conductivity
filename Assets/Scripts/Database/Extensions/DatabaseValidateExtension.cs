using System.Collections.Generic;

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
            new RuleForEdgeTemperatures(),
            new RuleForTimesOrder()
        };

        public static bool Validate(this Database database, out string error)
        {
            var isValid = true;
            var errors = new List<string>();

            foreach (var rule in _validateRules)
            {
                if (!rule.Validate(database, out string ruleError))
                {
                    isValid = false;
                    errors.Add(ruleError);
                }
            }

            error = string.Join("\n", errors);

            return isValid;
        }
    }
}