using System.Text.Json.Serialization;

namespace Core.Models
{
    public class Location : BaseModel
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Address { get; set; }
        [JsonIgnore]
        public virtual ICollection<InventoryItem>? InventoryItems { get; set; }
    }
}