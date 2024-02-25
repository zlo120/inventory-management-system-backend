namespace Core.Models
{
    public class User : BaseModel
    {
        public string Email { get; set; }
        public int GroupId { get; set; }
        public bool UserCreatedPassword { get; set; } = false;
        public string? RandomlyGeneratedPassword { get; set; }
    }
}