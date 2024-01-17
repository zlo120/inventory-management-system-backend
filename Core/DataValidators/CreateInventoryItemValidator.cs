using System.ComponentModel.DataAnnotations;

namespace Core.DataValidators
{
    public class CreateInventoryItemValidator
    {
        [Required]
        public string Serial { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Supplier { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public string Notes { get; set; }
        public bool? ALVLP { get; set; }
        public bool? UL { get; set; }
        public bool? MDM { get; set; }
        public bool? Reset { get; set; }
        public bool? GTG { get; set; }
    }
}
