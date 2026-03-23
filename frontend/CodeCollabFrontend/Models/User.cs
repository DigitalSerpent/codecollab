using System.ComponentModel.DataAnnotations;

namespace CodeCollabFrontend.Models;

public class User
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Имя обязательно")]
    [StringLength(50)]
    public string Name { get; set; } = "";
    
    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    public string Email { get; set; } = "";
    
    [Required(ErrorMessage = "Пароль обязателен")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен быть от 6 до 100 символов")]
    public string PasswordHash { get; set; } = "";
    
    public string? Avatar { get; set; } = "👤";
    public string? Cursor { get; set; } = "⬤";
    
    public ICollection<RoomParticipant>? RoomParticipants { get; set; }
}