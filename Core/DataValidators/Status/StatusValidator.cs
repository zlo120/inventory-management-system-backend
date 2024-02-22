using System.ComponentModel.DataAnnotations;

namespace Core.DataValidators.Status
{
    public class StatusValidator
    {
        [Required]
        public string Name { get; set; }
        public string? ColourCode { get; set; }
    }
}
