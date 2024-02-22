using System.ComponentModel.DataAnnotations;

namespace Core.DataValidators.Status
{
    public class UpdateStatusValidator
    {
        [Required]
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? ColourCode { get; set; }

    }
}
