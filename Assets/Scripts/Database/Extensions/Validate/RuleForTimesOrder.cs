namespace DatabaseLibrary
{
    internal class RuleForTimesOrder : IValidateRule
    {
        public bool Validate(Database database, out string error)
        {
            if (!IsSorted(database.Times))
            {
                error = "Times not ordered";

                return false;
            }
            else
            {
                error = string.Empty;

                return true;
            }
        }

        private static bool IsSorted(double[] array)
        {
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1] > array[i])
                    return false;
            }

            return true;
        }
    }
}
