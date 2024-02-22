using System.ComponentModel.DataAnnotations;

namespace Core.DataValidators.Inventory
{
    public class InsertInventoryValidator
    {
        [Required]
        public List<InventoryValidator> InventoryItems { get; set; }
    }
}