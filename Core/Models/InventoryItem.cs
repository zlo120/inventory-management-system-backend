namespace Core.Models
{
    public class InventoryItem : BaseModel
    {
        public string Serial { get; set; }
        public string Name { get; set; }
        public string Supplier { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
        public string Notes { get; set; }
        public bool ALVLP { get; set; }
        public bool UL { get; set; }
        public bool MDM { get; set; }
        public bool Reset { get; set; }
        public bool GTG { get; set; }
    }
}
