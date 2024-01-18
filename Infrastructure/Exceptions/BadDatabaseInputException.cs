namespace Infrastructure.Exceptions
{
    [Serializable]
    public class BadDatabaseInputException : Exception
    {
        public BadDatabaseInputException() { }

        public BadDatabaseInputException(string message)
            : base(message) { }

        public BadDatabaseInputException(string message, Exception inner)
            : base(message, inner) { }
    }
}
