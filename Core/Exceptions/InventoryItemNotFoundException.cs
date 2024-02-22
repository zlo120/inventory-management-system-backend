namespace Core.Exceptions
{
    public class InventoryItemNotFoundException : Exception
    {
        public InventoryItemNotFoundException() { }

        public InventoryItemNotFoundException(string message)
            : base(message) { }

        public InventoryItemNotFoundException(string message, Exception inner)
            : base(message, inner) { }
    }
}
