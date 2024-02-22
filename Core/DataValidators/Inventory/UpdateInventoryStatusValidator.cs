using System.ComponentModel.DataAnnotations;

namespace Core.DataValidators.Inventory
{
    public class UpdateInventoryStatusValidator
    {
        [Required]
        public int InventoryId { get; set; }
        [Required]
        public int StatusId { get; set; }
    }
}
