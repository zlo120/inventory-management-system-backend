using Newtonsoft.Json;

namespace Core.Models
{
    public class Status : BaseModel
    {
        public string Name { get; set; }
        public string ColourCode { get; set; }
        [JsonIgnore]
        public virtual ICollection<InventoryItem>? InventoryItems { get; set; }
    }
}