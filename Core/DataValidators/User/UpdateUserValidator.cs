using System.ComponentModel.DataAnnotations;

namespace Core.DataValidators.User
{
    public class UpdateUserValidator
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}