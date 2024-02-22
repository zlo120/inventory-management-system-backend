using System.ComponentModel.DataAnnotations;

namespace Core.DataValidators.Inventory
{
    public class TransferInventoryValidator
    {
        [Required]
        public int InventoryID { get; set; }
        [Required]
        public int LocationID { get; set; }
    }
}
