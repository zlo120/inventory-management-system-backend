using System.ComponentModel.DataAnnotations;

namespace Core.DataValidators.User
{
    public class UpdateUserGroupValidator
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int GroupId { get; set; }
    }
}