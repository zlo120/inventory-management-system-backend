using System.ComponentModel.DataAnnotations;

namespace Core.DataValidators.Location
{
    public class CreateLocationValidator
    {
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public string Address { get; set; }
    }
}
