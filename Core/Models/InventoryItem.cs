using Newtonsoft.Json;

namespace Core.Models
{
    public class InventoryItem : BaseModel
    {
        public string Serial { get; set; }
        public string Name { get; set; }
        public string Supplier { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
        public string? Notes { get; set; }
        public bool? ALVLP { get; set; }
        public bool? UL { get; set; }
        public bool? MDM { get; set; }
        public bool? Reset { get; set; }
        public bool? GTG { get; set; }
        public int LocationId { get; set; }

        [JsonIgnore]
        public virtual Location Location { get; set; }
        public int StatusId { get; set; }
        [JsonIgnore]
        public virtual Status Status { get; set; }
    }
}