namespace DatabaseLibrary
{
    internal interface IValidateRule
    {
        public bool Validate(Database database, out string error);
    }
}
