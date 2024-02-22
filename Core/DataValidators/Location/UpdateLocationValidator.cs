using System.ComponentModel.DataAnnotations;

namespace Core.DataValidators.Location
{
    public class UpdateLocationValidator
    {
        [Required]
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Address { get; set; }
    }
}
