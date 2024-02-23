using System.ComponentModel.DataAnnotations;

namespace Core.DataValidators.User
{
    public class RefreshValidator
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
