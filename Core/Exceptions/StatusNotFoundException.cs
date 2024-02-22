namespace Core.Exceptions
{
    public class StatusNotFoundException : Exception
    {
        public StatusNotFoundException() { }

        public StatusNotFoundException(string message)
            : base(message) { }

        public StatusNotFoundException(string message, Exception inner)
            : base(message, inner) { }
    }
}
