using System.ComponentModel.DataAnnotations;

namespace Core.DataValidators
{
    public class InsertInventoryValidator
    {
        [Required]
        public List<InventoryValidator> InventoryItems { get; set; }
    }
}